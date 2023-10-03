﻿using System;

using GP = BRIDGES.Solvers.GuidedProjection;
using Euc3D = BRIDGES.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;

using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;

using Types_GPA = Solvers.Types.GPA;
using Params_GPA = Solvers.Parameters.GPA;


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
            pManager.AddParameter(new Params_GPA.Param_Model(), "GPA Model", "M", "Assembled Model for the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddIntegerParameter("Iteration", "I", "Number of iteration of the solver.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_GPA.Param_Model(), "GPA Model", "M", "Model for the Guided Projection Algorithm", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            // ----- Initialisation ----- //

            Types_GPA.Gh_Model gh_Model = new Types_GPA.Gh_Model();
            int maxIteration = 0;


            // ----- Get Inputs ----- //

            if (!DA.GetData(0, ref gh_Model)) { return; }
            if(gh_Model.Value is null)
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, "Input parameter M failed to collect data.");
                DA.SetData(0, new Types_GPA.Gh_Model());
                return;
            }

            if (!DA.GetData(1, ref maxIteration)) { return; }
                        
            // ----- Core ----- //

            gh_Model.Value.MaxIteration = maxIteration;

            gh_Model.Value.InitialiseX();

            for (int i = 0; i < gh_Model.Value.MaxIteration; i++)
            {
                gh_Model.Value.RunIteration(false);
            }

            // ----- Set Output ----- //

            DA.SetData(0, gh_Model);
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
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }
}
