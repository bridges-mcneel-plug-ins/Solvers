using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;

using S_Types = Solvers.Types.GPA;
using S_Param = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA
{
    /// <summary>
    /// A grasshopper component disassembling a <see cref="S_Types.Gh_VariableSet"/> into its <see cref="double"/> variables.
    /// </summary>
    public class Comp_DeconstructDoubleVariableSet : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_DeconstructDoubleVariableSet"/> class.
        /// </summary>
        public Comp_DeconstructDoubleVariableSet()
          : base("Deconstruct Set - Double", "D Set",
              "Deconstruct a Variable Set into its numeric variables.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new S_Param.Param_VariableSet(), "Variable Set", "S", "Variable set to disassemble into numerical values.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Values", "V", "Numeric values contained in the variable set.", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/

            S_Types.Gh_VariableSet gh_Set = null;

            /******************** Get Inputs ********************/

            if (!DA.GetData(0, ref gh_Set)) { return; };

            /******************** Core ********************/
                        
            List<double> components = new List<double>(gh_Set.Count);
            for (int i = 0; i < gh_Set.Count; i++)
            {
                GP.Variable variable = gh_Set[i];

                components.Add(variable[0]);
            }
            
            /******************** Set Output ********************/

            DA.SetDataList(0, components);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3EB3BB1C-4C4A-4E3B-9944-3FE7C1DC32B1}"); }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure
        {
            get { return (GH_Kernel.GH_Exposure)TabExposure.Variable; }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }
}
