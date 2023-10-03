using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;
using GH_Data = Grasshopper.Kernel.Data;
using GH_Types = Grasshopper.Kernel.Types;

using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;

using Types_GPA = Solvers.Types.GPA;
using Params_GPA = Solvers.Parameters.GPA;
using Rhino.Geometry;


namespace Solvers.Components.GPA.Variable
{
    /// <summary>
    /// A grasshopper component creating a <see cref="Types_GPA.Gh_VariableSet"/>.
    /// </summary>
    public class Comp_DeconstructSet : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_DeconstructSet"/> class.
        /// </summary>
        public Comp_DeconstructSet()
          : base("Deconstruct Set", "Set",
              "Deconstruct a set of variables into its numerical values.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Set Name", "N", "Name of the set of variables to deconstruct", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_GPA.Param_Model(), "GPA Model", "M", "Assembled Model for the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Numerical Values", "N", "Numerical value representation of the variables in the specified set.", GH_Kernel.GH_ParamAccess.tree);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            // ----- Initialise ----- //

            string name = "";
            Types_GPA.Gh_Model model = new Types_GPA.Gh_Model();

            // ----- Get Inputs ----- //

            DA.GetData(0, ref name);
            DA.GetData(1, ref model);

            // ----- Core ----- //

            Grasshopper.DataTree<double> tree = new Grasshopper.DataTree<double>();

            if (model.Sets.TryGetValue(name, out List<GP.Variable> variables))
            {
                for (int i = 0; i < variables.Count; i++)
                {
                    double[] array = variables[i].ToArray();
                    tree.AddRange(array, new GH_Data.GH_Path(i));
                }
            }
            else
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error, "The specified name does not correspond to any variable set i the model.");
                return;
            }

            // ----- Set Output ----- //

            DA.SetDataTree(0, tree);

        }

        #endregion

        #region Override : GH_DocumentObject

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{87C3EAC8-D980-4D05-B54D-3E10FCC628C5}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Variable;


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }
}
