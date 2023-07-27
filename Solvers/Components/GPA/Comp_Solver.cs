using System;

using Euc3D = BRIDGES.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;

using Param = Solvers.Parameters;


namespace Solvers.Components.GPA
{
    /// <summary>
    /// A grasshopper component representing the solver for the Guided Projection Algorithm from its coordinates.
    /// </summary>
    public class Comp_Solver : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Solver"/> class.
        /// </summary>
        public Comp_Solver()
          : base("Guided Projection Algorithm Solver", "GPA",
              "Solver for the Guided Projection Algorithm described by Tang et al. in \"Form-finding with Polyhedral Meshes Made Simple\".",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Iteration", "I", "Number of Iteration of the solver.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/
            double x = 0d, y = 0d, z = 0d;


            /******************** Get Inputs ********************/
            if (!DA.GetData(0, ref x)) { return; }
            if (!DA.GetData(1, ref y)) { return; }
            if (!DA.GetData(2, ref z)) { return; }


            /******************** Core ********************/

            Euc3D.Point point = new Euc3D.Point(x, y, z);

            /******************** Set Output ********************/
            DA.SetData(0, point);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid
        {
            get { return new Guid("{4AEF4763-83CD-4C49-A80B-B5F5A728DC89}"); }
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
            get { return (GH_Kernel.GH_Exposure)TabExposure.Solver; }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new ComponentAttributes(this);
        }

        #endregion
    }
}
