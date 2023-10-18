using System;

using Euc3D = BRIDGES.Geometry.Euclidean3D;
using He = BRIDGES.DataStructures.PolyhedralMeshes.HalfedgeMesh;
using GP = BRIDGES.Solvers.GuidedProjection;
using LinAlg_Vect = BRIDGES.LinearAlgebra.Vectors;

using RH_Geo = Rhino.Geometry;

using GH_Kernel = Grasshopper.Kernel;

using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;
using Gh_Param_Euc3D = BRIDGES.McNeel.Grasshopper.Parameters.Geometry.Euclidean3D;
using Gh_Types_Euc3D = BRIDGES.McNeel.Grasshopper.Types.Geometry.Euclidean3D;
using System.Collections.Generic;
using BRIDGES.McNeel.Rhino.Extensions.Geometry.Euclidean3D;

namespace Solvers.Components.GPA.Energy
{

    #region Component Hexagonal Mesh

    /// <summary>
    /// A grasshopper component giving the fan orientation in a mesh.
    /// </summary>
    public class Comp_HexagonalMesh : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_FanOrientation"/> class.
        /// </summary>
        public Comp_HexagonalMesh()
          : base("Hexagonal Mesh", "Hexa",
              "Creates an hexagonal halfedge mesh from faces as polylines.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polylines", "P", "Polylines representing the faces of the hewagonal mesh", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Gh_Param_Euc3D.Param_HeMesh(), "Mesh", "M", "Halfedge mesh.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/
            List<RH_Geo.Curve> curves = new List<RH_Geo.Curve>();

            /******************** Get Inputs ********************/

            if (!DA.GetDataList(0, curves)) { return; };

            /******************** Core ********************/

            He.Mesh<Euc3D.Point> mesh = new He.Mesh<Euc3D.Point>();

            foreach(RH_Geo.Curve curve in curves)
            {
                curve.TryGetPolyline(out RH_Geo.Polyline polyline);

                int vertexCount = polyline.IsClosed ? polyline.Count - 1 : polyline.Count;

                List<He.Vertex<Euc3D.Point>> faceVertices = new List<He.Vertex<Euc3D.Point>>(vertexCount);
                for (int i = 0; i < vertexCount; i++)
                {
                    polyline[i].CastTo(out Euc3D.Point point);

                    He.Vertex<Euc3D.Point> faceVertex = null;
                    foreach (He.Vertex<Euc3D.Point> vertex in mesh.GetVertices())
                    {
                        if(vertex.Position.DistanceTo(point) < 1e-4) { faceVertex = vertex; break; }
                    }

                    if (faceVertex is null) { faceVertex = mesh.AddVertex(point); }

                    faceVertices.Add(faceVertex);
                }

                mesh.AddFace(faceVertices);

            }

            /******************** Set Output ********************/

            DA.SetData(0, mesh);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{AA7DDB73-BC5D-4BB8-A7BA-A42AECB2A8B2}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Nexorade;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }

    #endregion

    #region Component Fan Orientation

    /// <summary>
    /// A grasshopper component giving the fan orientation in a mesh.
    /// </summary>
    public class Comp_FanOrientation : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_FanOrientation"/> class.
        /// </summary>
        public Comp_FanOrientation()
          : base("Fan Orientation", "Fan Ori.",
              "Gives the orientation of the fans in a mesh.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Gh_Param_Euc3D.Param_HeMesh(), "Mesh", "M", "Halfedge mesh.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Fan Orientation", "O", "Fan Orientation : 1 = Left; -1 = Right; 0 = Not Assigned. All the fans should be assigned.", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Initialisation ********************/

            Gh_Types_Euc3D.Gh_HeMesh gh_Mesh = null;

            /******************** Get Inputs ********************/

            if (!DA.GetData(0, ref gh_Mesh)) { return; };
            He.Mesh<Euc3D.Point> heMesh = gh_Mesh.Value;

            /******************** Core ********************/

            int vertexCount = heMesh.VertexCount;

            // Fan Orientation : 1 = Left; -1 = Right; 0 = Not Assigned
            int[] fanOrientation = new int[vertexCount];

            foreach(He.Vertex<Euc3D.Point> v in heMesh.GetVertices())
            {
                if(v.OutgoingHalfedge is null) { heMesh.RemoveVertex(v); }
            }

            He.Vertex<Euc3D.Point> v_First = heMesh.GetVertex(0);
            He.Halfedge<Euc3D.Point> he_First = v_First.OutgoingHalfedge;
            fanOrientation[he_First.StartVertex.Index] = 1;
            fanOrientation[he_First.EndVertex.Index] = -1;

            SpreadOrientation(he_First);

            // Halfedge whose:
            // - Start Vertex is being used for propagation (turning around it)
            // - End Vertex is where the propagation comes from (i.e. its fan is already assigned)
            void SpreadOrientation(He.Halfedge<Euc3D.Point> halfedge)
            {
                int orientation = fanOrientation[halfedge.EndVertex.Index];

                He.Halfedge<Euc3D.Point> he = halfedge.PrevHalfedge.PairHalfedge;

                while (he != halfedge)
                {
                    if (fanOrientation[he.EndVertex.Index] == 0)
                    {
                        fanOrientation[he.EndVertex.Index] = orientation;
                        SpreadOrientation(he.PairHalfedge);
                    }

                    he = he.PrevHalfedge.PairHalfedge;
                }
            }

            /******************** Set Output ********************/

            DA.SetDataList(0, fanOrientation);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{FE32D8D4-F03F-4CAC-B651-3D906B64DD36}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Nexorade;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }

    #endregion

    #region Component

    /// <summary>
    /// A grasshopper component building the energies for the nexorade.
    /// </summary>
    public class Comp_Nexorade : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Nexorade"/> class.
        /// </summary>
        public Comp_Nexorade()
          : base("Nexorade", "Nexorade",
              "Creates the Nexorade Energies for the GPA Solver.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Gh_Param_Euc3D.Param_HeMesh(), "Mesh", "M", "Halfedge mesh.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fan Orientation", "O", "Fan Orientation : 1 = Left; -1 = Right; 0 = Not Assigned. All the fans should be assigned.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddNumberParameter("Eccentricity", "E", "Eccentricity value for the fans.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Engagement Length", "L", "Engagement Length value for the fans.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            // ----- Initialisation ----- //

            Gh_Types_Euc3D.Gh_HeMesh gh_Mesh = null;
            List<int> fanOrientation = new List<int>();
            double eccentricity = 0d;
            double engagementLength = 0d;

            // ----- Get Inputs ----- //

            if (!DA.GetData(0, ref gh_Mesh)) { return; };
            He.Mesh<Euc3D.Point> heMesh = gh_Mesh.Value;

            if (!DA.GetDataList(1, fanOrientation)) { return; };
            if (!DA.GetData(2, ref eccentricity)) { return; };
            if (!DA.GetData(3, ref engagementLength)) { return; };

            // ----- Core ----- //

            /* Foreach halfedge 
             * Create Eccentricity constraint - BEWARE : Not two edge can be on the border
             * Create EngagementLength constraint - BEWARE : middle edge not on the border
             * 
             * 
             */


            // ----- Set Output ----- //

            //DA.SetDataList(0, fanOrientation);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{0DF18FA0-FF3C-4710-AA7B-5A92A1C5A40F}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Nexorade;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }

    #endregion

    #region Energies & Constraints

    /// <summary>
    /// Energy enforcing two initially co-planar three-dimensional euclidean ray to have a fixed eccentricity. The list of variables of this energy consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>T<sub>i</sub></term>
    ///         <description> Variable representing the translation vector of the first ray.</description>
    ///     </item>
    ///     <item> 
    ///         <term>T<sub>j</sub></term>
    ///         <description> Variable representing the translation vector of the second ray.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class Eccentricity : GP.Abstracts.EnergyType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Eccentricity"/> class.
        /// </summary>
        /// <param name="axis">  Axis of the first ray.</param>
        /// <param name="otherAxis"> Axis of the second ray. </param>
        /// <param name="eccentricity"> Target eccentricity between the three-dimensional euclidean ray. </param>
        /// <remarks> The ray axes should be "outgoing" ... </remarks>
        public Eccentricity(Euc3D.Vector axis, Euc3D.Vector otherAxis, double eccentricity)
        {
            // ----- Define Ki ----- //

            int[] rowIndices = new int[6] { 0, 1, 2, 3, 4, 5 };

            Euc3D.Vector crossProduct = Euc3D.Vector.CrossProduct(axis, otherAxis); crossProduct.Unitise();
            double[] values = new double[6]
            {
                - crossProduct.X, - crossProduct.Y, - crossProduct.Z,
                crossProduct.X, crossProduct.Y, crossProduct.Z,
            };

            LocalKi = new LinAlg_Vect.SparseVector(6, rowIndices, values);

            // ----- Define Si ----- //

            Si = eccentricity;
        }

        #endregion
    }

    /// <summary>
    /// Energy enforcing two initially co-planar three-dimensional euclidean ray to have a fixed length along a third middle ray (also contained in the inital plane). The list of variables of this energy consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>T<sub>r</sub></term>
    ///         <description> Variable representing the translation vector of the right ray.</description>
    ///     </item>
    ///     <item> 
    ///         <term>T<sub>m</sub></term>
    ///         <description> Variable representing the translation vector of the middle ray.</description>
    ///     </item>
    ///     <item> 
    ///         <term>T<sub>l</sub></term>
    ///         <description> Variable representing the translation vector of the left ray.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class EngagementLength : GP.Abstracts.EnergyType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Eccentricity"/> class.
        /// </summary>
        /// <param name="rightAxis">  Axis of the right ray.</param>
        /// <param name="middleAxis"> Axis of the middle ray. </param>
        /// <param name="leftAxis"> Axis of the left ray. </param>
        /// <param name="length"> Target engagement length.. </param>
        /// <remarks> For left fans, the rays should represent ougoing directions. Otherwise, the ray axes should be fliped for right fans. </remarks>
        public EngagementLength(Euc3D.Vector rightAxis, Euc3D.Vector middleAxis, Euc3D.Vector leftAxis, double length)
        {
            // ----- Define Ki ----- //

            int[] rowIndices = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            rightAxis.Unitise(); middleAxis.Unitise(); leftAxis.Unitise();
            double[] values = new double[9];

            double sl_Right = (rightAxis * rightAxis);
            double sl_Middle = (middleAxis * middleAxis), l_Middle = Math.Sqrt(sl_Middle);
            double sl_Left = (leftAxis * leftAxis);

            double dot_MiddleRight = (middleAxis * rightAxis);
            double dot_MiddleLeft = (middleAxis * leftAxis);

            double d_MiddleRight = (sl_Middle * sl_Right) - (dot_MiddleRight * dot_MiddleRight); d_MiddleRight /= l_Middle;
            double d_MiddleLeft = (sl_Middle * sl_Left) - (dot_MiddleLeft * dot_MiddleLeft); d_MiddleLeft /= l_Middle;

            double f_Right = dot_MiddleRight / d_MiddleRight;
            double f_MiddleRight = sl_Right / d_MiddleRight;
            double f_MiddleLeft = sl_Left / d_MiddleLeft;
            double f_Left = dot_MiddleLeft / d_MiddleLeft;

            // For X coordinates
            double tmp_MiddleRight = (f_MiddleRight * middleAxis.X) - (f_Right * rightAxis.X);
            double tmp_MiddleLeft = (f_Left * leftAxis.X) - (f_MiddleLeft * middleAxis.X);
            values[0] = tmp_MiddleRight;
            values[3] = -(tmp_MiddleLeft + tmp_MiddleRight);
            values[6] = tmp_MiddleLeft;

            // For Y coordinates
            tmp_MiddleRight = (f_MiddleRight * middleAxis.Y) - (f_Right * rightAxis.Y);
            tmp_MiddleLeft = (f_Left * leftAxis.Y) - (f_MiddleLeft * middleAxis.Y);
            values[1] = tmp_MiddleRight;
            values[4] = -(tmp_MiddleLeft + tmp_MiddleRight);
            values[7] = tmp_MiddleLeft;

            // For Z coordinates
            tmp_MiddleRight = (f_MiddleRight * middleAxis.Z) - (f_Right * rightAxis.Z);
            tmp_MiddleLeft = (f_Left * leftAxis.Z) - (f_MiddleLeft * middleAxis.Z);
            values[2] = tmp_MiddleRight;
            values[5] = -(tmp_MiddleLeft + tmp_MiddleRight);
            values[8] = tmp_MiddleLeft;

            LocalKi = new LinAlg_Vect.SparseVector(9, rowIndices, values);

            // ----- Define Si ----- //

            Si = length;
        }

        #endregion
    }

    #endregion
}
