using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using Gh_Types_Euc3D = BRIDGES.McNeel.Grasshopper.Types.Geometry.Euclidean3D;
using Gh_Param_Euc3D = BRIDGES.McNeel.Grasshopper.Parameters.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;

using Types_GPA = Solvers.Types.GPA;
using Params_GPA = Solvers.Parameters.GPA;


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
            pManager.AddParameter(new Params_GPA.Param_QuadraticConstraint(), "Constraints", "C", "Constraints", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_GPA.Param_Model(), "Model", "M", "Model for the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/

            List<Types_GPA.Gh_Set> gh_Sets = null;
            List<Types_GPA.Gh_Energy> gh_Energies = null;
            List<Types_GPA.Gh_QuadraticConstraint> gh_Constraints = null;

            /******************** Get Inputs ********************/

            if (!DA.GetDataList(0, gh_Sets)) { return; };
            if (!DA.GetDataList(1, gh_Energies)) { return; };
            if (!DA.GetDataList(2, gh_Constraints)) { return; };

            /******************** Core ********************/

            GP.GuidedProjectionAlgorithm gpa = new GP.GuidedProjectionAlgorithm(1e-8, 100);

            Dictionary<Guid, GP.VariableSet> dict_Set = new Dictionary<Guid, GP.VariableSet>(gh_Sets.Count);
            foreach (Types_GPA.Gh_Set gh_Set in gh_Sets)
            {
                GP.VariableSet set = gpa.AddVariableSet(gh_Set.VariableCount, gh_Set.VariableCount);

                for (int i = 0; i < gh_Set.VariableCount; i++)
                {
                    set.AddVariable(gh_Set.GetVariable(i));
                }

                dict_Set.Add(gh_Set.GUID, set);
            }

            foreach (Types_GPA.Gh_Energy gh_Energy in gh_Energies)
            {
                int count = gh_Energy.Variables.Count;

                List<(GP.VariableSet, int)> variables = new List<(GP.VariableSet, int)>(count);
                for (int i = 0; i < count; i++)
                {
                    Types_GPA.Gh_Variable gh_Variable = gh_Energy.Variables[i];
                    variables.Add((dict_Set[gh_Variable.SetGuid], gh_Variable.Index));
                    
                }

                // To Do : Manage if silent variables are added :
                // - set needs to be added to the model
                // - variable needs to be added to to the energy variables

                gpa.AddEnergy(gh_Energy.Type, variables, gh_Energy.Weight);
            }


            foreach (Types_GPA.Gh_QuadraticConstraint gh_Constraint in gh_Constraints)
            {
                int count = gh_Constraint.Variables.Count;

                List<(GP.VariableSet, int)> variables = new List<(GP.VariableSet, int)>(count);
                for (int i = 0; i < count; i++)
                {
                    Types_GPA.Gh_Variable gh_Variable = gh_Constraint.Variables[i];
                    variables.Add((dict_Set[gh_Variable.SetGuid], gh_Variable.Index));

                }

                // To Do :  Manage if silent variables are added :
                // - set needs to be added to the model
                // - variable needs to be added to to the energy variables


                gpa.AddConstraint(gh_Constraint.Type, variables, gh_Constraint.Weight);
            }



            /******************** Set Output ********************/

            DA.SetData(0, gpa);
        }

        #endregion


        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{0DB58DA4-D469-4AF7-AC13-D9B630127B42}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Solver;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new ComponentAttributes(this);
        }

        #endregion
    }
}
