using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;
using Euc3D = BRIDGES.Geometry.Euclidean3D;

using Param_Euc3D = BRIDGES.McNeel.Grasshopper.Parameters.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;

using Typ = Solvers.Types.GPA;
using Param = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA
{
    /// <summary>
    /// A grasshopper component disassembling a <see cref="GP.VariableSet"/> into its <see cref="Euc3D.Point"/> variables.
    /// </summary>
    public class Comp_DeconstructPointVariableSet : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_DeconstructPointVariableSet"/> class.
        /// </summary>
        public Comp_DeconstructPointVariableSet()
          : base("Deconstruct Set - Point", "P Set",
              "Deconstruct a Variable Set into its point variables.",
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
            pManager.AddParameter(new Param_Euc3D.Param_Point(Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.Parameters]),
                "Points", "P", "Points contained in the variable set.", GH_Kernel.GH_ParamAccess.list);
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
            int variableDimension = set.VariableDimension;

            List<Euc3D.Point> variables = new List<Euc3D.Point>(variableCount);
            for (int i = 0; i < variableCount; i++)
            {
                double[] variableComponents = set.GetVariable(i);
                variables.Add(new Euc3D.Point(variableComponents));
            }

            /******************** Set Output ********************/

            DA.SetDataList(0, variables);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1B0E17D2-08C9-4612-9ED6-2A3D9BB48D85}"); }
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
