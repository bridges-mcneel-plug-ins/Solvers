using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;
using Euc3D = BRIDGES.Geometry.Euclidean3D;

using Param_Euc3D = BRIDGES.McNeel.Grasshopper.Parameters.Geometry.Euclidean3D;

using GH = Grasshopper;
using GH_Kernel = Grasshopper.Kernel;
using GH_Types = Grasshopper.Kernel.Types;

using Typ = Solvers.Types.GPA;
using Param = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA
{
    /// <summary>
    /// A grasshopper component disassembling a <see cref="GP.VariableSet"/> into its variables.
    /// </summary>
    public class Comp_DeconstructGenericVariableSet : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_DeconstructGenericVariableSet"/> class.
        /// </summary>
        public Comp_DeconstructGenericVariableSet()
          : base("Deconstruct Set - Generic", "G Set",
              "Deconstruct a Variable Set consisting of variables.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion

        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param.Param_VariableSet(), "Variable Set", "G", "Variable set to disassemble into values.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Variable Components", "C", "Variable components contained in the variable set.", GH_Kernel.GH_ParamAccess.tree);
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

            GH.DataTree<double> variables = new GH.DataTree<double>();
            for (int i = 0; i < variableCount; i++)
            {
                double[] variableComponents = set.GetVariable(i);
                variables.AddRange(variableComponents, new GH_Kernel.Data.GH_Path(i));
            }

            /******************** Set Output ********************/

            DA.SetDataTree(0, variables);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid
        {
            get { return new Guid("{F3EE0C75-C195-4254-A2BD-FA5ECA9E1014}"); }
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
