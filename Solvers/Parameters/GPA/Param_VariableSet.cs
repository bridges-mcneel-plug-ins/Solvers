using System;

using GH_Kernel = Grasshopper.Kernel;

using Types_GPA = Solvers.Types.GPA;


namespace Solvers.Parameters.GPA
{
    /// <summary>
    /// A <see cref="Types_GPA.Gh_Set"/> grasshopper parameter.
    /// </summary>
    public class Param_VariableSet : GH_Kernel.GH_Param<Types_GPA.Gh_Set>
    {
        #region Properties

        /// <inheritdoc cref="GH_Kernel.IGH_PreviewObject.Hidden"/>
        public bool Hidden => true;

        /// <inheritdoc cref="GH_Kernel.IGH_PreviewObject.IsPreviewCapable"/>
        public bool IsPreviewCapable => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Param_VariableSet"/>.
        /// </summary>
        public Param_VariableSet()
          : base("Set of Variables", "Set", "Contains a collection of sets of variables for the Guided Projection Algorithm.",
                Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.Parameters], GH_Kernel.GH_ParamAccess.item)
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{FD42CE01-3191-40FA-9784-D2A1520DD1E7}");

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
