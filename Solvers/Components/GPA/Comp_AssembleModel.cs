using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using Gh_Types_Euc3D = BRIDGES.McNeel.Grasshopper.Types.Geometry.Euclidean3D;
using Gh_Param_Euc3D = BRIDGES.McNeel.Grasshopper.Parameters.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;

using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;

using Types_GPA = Solvers.Types.GPA;
using Params_GPA = Solvers.Parameters.GPA;
using Grasshopper.Documentation;


namespace Solvers.Components.GPA
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.GuidedProjectionAlgorithm"/> model.
    /// </summary>
    public class Comp_AssembleModel : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_AssembleModel"/> class.
        /// </summary>
        public Comp_AssembleModel()
          : base("Assemble Guided Projection Model", "Assemble GP",
              "Create a model for the Guided Projection Algorithm (GPA).",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_GPA.Param_VariableSet(), "Sets", "S", "Sets of variable used by the energies and constraints.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Params_GPA.Param_Energy(), "Energies", "E", "Energies", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Params_GPA.Param_Constraint(), "Constraints", "C", "Constraints", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_GPA.Param_Model(), "Model", "M", "Model for the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            // ---------- Initialisation ---------- //

            List<Types_GPA.Gh_VariableSet> gh_Sets = null;
            List<Types_GPA.Gh_Energy> gh_Energies = null;
            List<Types_GPA.Gh_Constraint> gh_Constraints = null;

            // ---------- Get Inputs ---------- //

            if (!DA.GetDataList(0, gh_Sets)) { return; };
            if (!DA.GetDataList(1, gh_Energies)) { return; };
            if (!DA.GetDataList(2, gh_Constraints)) { return; };

            // ---------- Core ---------- //

            int setVariableCount = 0;
            foreach (Types_GPA.Gh_VariableSet gh_Set in gh_Sets)
            {
                setVariableCount += gh_Set.Count;
            }

            GP.GuidedProjectionAlgorithm gpa = new GP.GuidedProjectionAlgorithm(1e-8, 100);


            // ---------- Variables ---------- //

            // Dictionary containing:
            // - Key : Variable defined by the user before the creation of the model.
            // - Value : Varriable defined from the gpa model.

            foreach(Types_GPA.Gh_VariableSet gh_Set in gh_Sets)
            {
                foreach(GP.Variable userVariable in gh_Set)
                {
                    bool isVariableAdded = gpa.TryAddVariable(userVariable);

                    if(!isVariableAdded)
                    {
                        this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, $"The set {gh_Set.Name} contains a variable that was already added to the model.");
                    }
                }
            }

            // ---------- Energies ---------- //

            foreach (Types_GPA.Gh_Energy gh_Energy in gh_Energies)
            {
                IReadOnlyList<GP.Variable> variables = gh_Energy.Value.Variables;
                foreach(GP.Variable variable in variables)
                {
                    bool isVariableAdded = gpa.TryAddVariable(variable);
                    if (isVariableAdded)
                    {
                        this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, $"A variable which does not belong to any set was added to the model. It might be a dummy variable.");
                    }
                }

                bool isEnergyAdded = gpa.TryAddEnergy(gh_Energy.Value);
                if (!isEnergyAdded)
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, $"This energy is a duplicate, it was already added to the model.");
                }
            }

            // ---------- Constraints ---------- //

            foreach (Types_GPA.Gh_Constraint gh_Constraint in gh_Constraints)
            {
                IReadOnlyList<GP.Variable> variables = gh_Constraint.Value.Variables;
                foreach (GP.Variable variable in variables)
                {
                    bool isVariableAdded = gpa.TryAddVariable(variable);
                    if (isVariableAdded)
                    {
                        this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, $"A variable which does not belong to any set was added to the model. It might be a dummy variable.");
                    }
                }

                bool isConstraintAdded = gpa.TryAddConstraint(gh_Constraint.Value);
                if (!isConstraintAdded)
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, $"This constraint is a duplicate, it was already added to the model.");
                }
            }

            /******************** Set Output ********************/

            Types_GPA.Gh_Model gh_Model = new Types_GPA.Gh_Model(gpa);
            DA.SetData(0, gpa);
        }

        #endregion

        #region Override : GH_DocumentObject

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{0DB58DA4-D469-4AF7-AC13-D9B630127B42}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Solver;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }
}
