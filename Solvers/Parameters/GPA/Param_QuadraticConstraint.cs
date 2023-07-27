using System;

using GH_Kernel = Grasshopper.Kernel;

using Types_GPA = Solvers.Types.GPA;


namespace Solvers.Parameters.GPA
{
    /// <summary>
    /// A <see cref="Types_GPA.Gh_QuadraticConstraint"/> grasshopper parameter.
    /// </summary>
    public class Param_QuadraticConstraint : GH_Kernel.GH_Param<Types_GPA.Gh_QuadraticConstraint>
    {
        #region Properties

        /// <inheritdoc cref="GH_Kernel.IGH_PreviewObject.Hidden"/>
        public bool Hidden => true;

        /// <inheritdoc cref="GH_Kernel.IGH_PreviewObject.IsPreviewCapable"/>
        public bool IsPreviewCapable => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Param_QuadraticConstraint"/>.
        /// </summary>
        public Param_QuadraticConstraint()
          : base("Quadratic Constraint", "Q. Constraint", "Contains a collection of quadratic constraints for the Guided Projection Algorithm.",
                Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.Parameters], GH_Kernel.GH_ParamAccess.item)
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{2259E43C-6756-4258-8130-DEAB8AACF5EB}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.GPA;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new ParameterAttributes(this);
        }

        #endregion

        #region Override : GH_ActiveObject

        /// <inheritdoc cref="GH_Kernel.GH_ActiveObject.Locked"/>
        public override bool Locked
        {
            get { return base.Locked; }
            set
            {
                if (base.Locked != value)
                {
                    base.Locked = value;
                }
            }
        }

        #endregion
    }
}