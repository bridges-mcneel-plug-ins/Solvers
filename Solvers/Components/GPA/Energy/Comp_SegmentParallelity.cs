using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;
using EnergyTypes = BRIDGES.Solvers.GuidedProjection.EnergyTypes;

using Gh_Types_Euc3D = BRIDGES.McNeel.Grasshopper.Types.Geometry.Euclidean3D;
using Gh_Params_Euc3D = BRIDGES.McNeel.Grasshopper.Parameters.Geometry.Euclidean3D;
using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;

using Types_GPA = Solvers.Types.GPA;
using Params_GPA = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA.Energy
{
    /// <summary>
    /// A grasshopper component creating a <see cref="EnergyTypes.SegmentParallelity"/>-based <see cref="GP.Energy"/>.
    /// </summary>
    public class Comp_SegmentParallelity : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_SegmentParallelity"/> class.
        /// </summary>
        public Comp_SegmentParallelity()
          : base("Segment Parallelity", "Parallel",
              "Create a Segment Parallelity energy for the Guided Projection Algorithm.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_GPA.Param_Variable(), "Start Variable", "S", "Variable representing the start of the segment", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_GPA.Param_Variable(), "End Variable", "E", "Variable representing the end of the segment", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_GPA.Param_Variable(), "Length", "L", "Segment Length (Complementary Variable)", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Gh_Params_Euc3D.Param_Vector(), "Vector", "V", "Vector with which the segment must be aligned.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Weight of the energy.", GH_Kernel.GH_ParamAccess.item);

            pManager[4].Optional = true;
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_GPA.Param_Variable(), "Variable", "V", "Variable at the given index.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/

            Types_GPA.Gh_Variable start = null;
            Types_GPA.Gh_Variable end = null;
            Types_GPA.Gh_Variable length = null;

            Gh_Types_Euc3D.Gh_Vector vector = null;
            double weight = 0.0;

            /******************** Get Inputs ********************/

            if (!DA.GetData(0, ref start)) { return; };
            if (!DA.GetData(1, ref end)) { return; };
            if (!DA.GetData(2, ref length)) { return; };

            if (!DA.GetData(3, ref vector)) { return; };
            if (!DA.GetData(4, ref weight)) { weight = 1.0; };

            /******************** Core ********************/

            int dimension = vector.Value.Dimension;
            if(dimension != start.Value.Dimension || dimension != end.Value.Dimension)
            {
                throw new ArgumentException("The start and end variables must have the same number of components than the vector.", new RankException());
            }

            double[] components = new double[] { vector.Value.X, vector.Value.Y, vector.Value.Z };
            EnergyTypes.SegmentParallelity energyType = new EnergyTypes.SegmentParallelity(components);

            GP.Variable[] variables = new GP.Variable[3] { start.Value, end.Value, length.Value };

            GP.Energy energy = new GP.Energy(energyType, variables, weight);
            Types_GPA.Gh_Energy gh_Energy = new Types_GPA.Gh_Energy(energy);

            /******************** Set Output ********************/

            DA.SetData(0, gh_Energy);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{4F5A0AE6-807B-4A65-A5E1-A7C848022A03}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Energy;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }
}
