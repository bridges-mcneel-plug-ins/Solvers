using System;

using GH_Kernel = Grasshopper.Kernel;

using S_Types = Solvers.Types.GPA;
using S_Param = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA.Variable
{
    /// <summary>
    /// A grasshopper component creating a <see cref="S_Types.Gh_Variable"/>.
    /// </summary>
    public class Comp_SetVariable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_SetVariable"/> class.
        /// </summary>
        public Comp_SetVariable()
          : base("Set Variable", "Variable",
              "Retrieve a specific variable from a set.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new S_Param.Param_VariableSet(), "Set of Variables", "S", "Set in which the variable is stored.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddIntegerParameter("Variable Index", "I", "Index of the variable in the set.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new S_Param.Param_Variable(), "Variable", "V", "Variable at the given index.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/

            S_Types.Gh_Set set = null;
            int index = 0;

            /******************** Get Inputs ********************/

            if (!DA.GetData(0, ref set)) { return; };
            if (!DA.GetData(1, ref index)) { return; };

            /******************** Core ********************/
            
            S_Types.Gh_Variable pair = new S_Types.Gh_Variable(set.Name, set.GUID, set.VariableCount, index);

            /******************** Set Output ********************/

            DA.SetData(0, pair);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{B74CCC1A-F1A1-4295-9EE5-E8E908DC5744}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Variable;

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
