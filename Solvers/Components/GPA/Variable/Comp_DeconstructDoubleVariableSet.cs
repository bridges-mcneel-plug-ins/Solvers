using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Typ = Solvers.Types.GPA;
using Param = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA
{
    /// <summary>
    /// A grasshopper component disassembling a <see cref="GP.VariableSet"/> into its <see cref="double"/> variables.
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
            pManager.AddParameter(new Param.Param_VariableSet(), "Variable Set", "S", "Variable set to disassemble into numerical values.", GH_Kernel.GH_ParamAccess.item);
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

            Typ.Gh_Set set = null;

            /******************** Get Inputs ********************/

            if (!DA.GetData(0, ref set)) { return; };

            /******************** Core ********************/

            int variableCount = set.VariableCount;

            List<double> components = new List<double>(variableCount);
            for (int i = 0; i < variableCount; i++)
            {
                components.Add(set.GetComponent(i));
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
            m_attributes = new ComponentAttributes(this);
        }

        #endregion
    }
}
