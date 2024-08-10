// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Utils.Revit.RevitProxy;

public enum ParameterGroupProxy
{
    /// <summary>"Life Safety"</summary>
    PG_LIFE_SAFETY = -5000232, // 0xFFB3B3D8
    /// <summary>"Electrical Analysis"</summary>
    PG_ELECTRICAL_ANALYSIS = -5000231, // 0xFFB3B3D9
    /// <summary>"Alternate Units"</summary>
    PG_ALTERNATE_UNITS = -5000230, // 0xFFB3B3DA
    /// <summary>"Primary Units"</summary>
    PG_PRIMARY_UNITS = -5000229, // 0xFFB3B3DB
    /// <summary>"Cross-Section Definition"</summary>
    PG_WALL_CROSS_SECTION_DEFINITION = -5000228, // 0xFFB3B3DC
    /// <summary>"Route Analysis"</summary>
    PG_ROUTE_ANALYSIS = -5000227, // 0xFFB3B3DD
    /// <summary>"Geolocation"</summary>
    PG_GEO_LOCATION = -5000226, // 0xFFB3B3DE
    /// <summary>"Structural Section Geometry"</summary>
    PG_STRUCTURAL_SECTION_GEOMETRY = -5000225, // 0xFFB3B3DF
    /// <summary>"Material Thermal Properties"</summary>
    PG_ENERGY_ANALYSIS_BLDG_CONS_MTL_THERMAL_PROPS = -5000221, // 0xFFB3B3E3
    /// <summary>"Room/Space Data"</summary>
    PG_ENERGY_ANALYSIS_ROOM_SPACE_DATA = -5000220, // 0xFFB3B3E4
    /// <summary>"Building Data"</summary>
    PG_ENERGY_ANALYSIS_BUILDING_DATA = -5000219, // 0xFFB3B3E5
    /// <summary>"Set"</summary>
    PG_COUPLER_ARRAY = -5000218, // 0xFFB3B3E6
    /// <summary>"Advanced"</summary>
    PG_ENERGY_ANALYSIS_ADVANCED = -5000217, // 0xFFB3B3E7
    /// <summary>"Releases / Member Forces"</summary>
    PG_RELEASES_MEMBER_FORCES = -5000216, // 0xFFB3B3E8
    /// <summary>"Secondary End"</summary>
    PG_SECONDARY_END = -5000214, // 0xFFB3B3EA
    /// <summary>"Primary End"</summary>
    PG_PRIMARY_END = -5000213, // 0xFFB3B3EB
    /// <summary>"Moments"</summary>
    PG_MOMENTS = -5000212, // 0xFFB3B3EC
    /// <summary>"Forces"</summary>
    PG_FORCES = -5000211, // 0xFFB3B3ED
    /// <summary>"Fabrication Product Data"</summary>
    PG_FABRICATION_PRODUCT_DATA = -5000210, // 0xFFB3B3EE
    /// <summary>"Reference"</summary>
    PG_REFERENCE = -5000208, // 0xFFB3B3F0
    /// <summary>"Geometric Position"</summary>
    PG_GEOMETRY_POSITIONING = -5000207, // 0xFFB3B3F1
    /// <summary>"Division Geometry"</summary>
    PG_DIVISION_GEOMETRY = -5000206, // 0xFFB3B3F2
    /// <summary>"Segments and Fittings"</summary>
    PG_SEGMENTS_FITTINGS = -5000205, // 0xFFB3B3F3
    /// <summary>"Extension (End/Top)"</summary>
    PG_CONTINUOUSRAIL_END_TOP_EXTENSION = -5000204, // 0xFFB3B3F4
    /// <summary>"Extension (Beginning/Bottom)"</summary>
    PG_CONTINUOUSRAIL_BEGIN_BOTTOM_EXTENSION = -5000203, // 0xFFB3B3F5
    /// <summary>"Winders"</summary>
    PG_STAIRS_WINDERS = -5000202, // 0xFFB3B3F6
    /// <summary>"Supports"</summary>
    PG_STAIRS_SUPPORTS = -5000201, // 0xFFB3B3F7
    /// <summary>"End Connection"</summary>
    PG_STAIRS_OPEN_END_CONNECTION = -5000200, // 0xFFB3B3F8
    /// <summary>"Handrail 2"</summary>
    PG_RAILING_SYSTEM_SECONDARY_FAMILY_HANDRAILS = -5000199, // 0xFFB3B3F9
    /// <summary>"Terminations"</summary>
    PG_TERMINATION = -5000198, // 0xFFB3B3FA
    /// <summary>"Threads/Risers"</summary>
    PG_STAIRS_TREADS_RISERS = -5000197, // 0xFFB3B3FB
    /// <summary>"Calculation Rules"</summary>
    PG_STAIRS_CALCULATOR_RULES = -5000196, // 0xFFB3B3FC
    /// <summary>"Dimensions     (linear units or % of thickness)"</summary>
    PG_SPLIT_PROFILE_DIMENSIONS = -5000195, // 0xFFB3B3FD
    /// <summary>"Length"</summary>
    PG_LENGTH = -5000194, // 0xFFB3B3FE
    /// <summary>"Nodes"</summary>
    PG_NODES = -5000193, // 0xFFB3B3FF
    /// <summary>"Analytical Properties"</summary>
    PG_ANALYTICAL_PROPERTIES = -5000192, // 0xFFB3B400
    /// <summary>"Analytical Alignment"</summary>
    PG_ANALYTICAL_ALIGNMENT = -5000191, // 0xFFB3B401
    /// <summary>"Rise / Drop"</summary>
    PG_SYSTEMTYPE_RISEDROP = -5000190, // 0xFFB3B402
    /// <summary>"Lining"</summary>
    PG_LINING = -5000189, // 0xFFB3B403
    /// <summary>"Insulation"</summary>
    PG_INSULATION = -5000188, // 0xFFB3B404
    /// <summary>"Overall Legend"</summary>
    PG_OVERALL_LEGEND = -5000187, // 0xFFB3B405
    /// <summary>"Visibility"</summary>
    PG_VISIBILITY = -5000186, // 0xFFB3B406
    /// <summary>"Supports"</summary>
    PG_SUPPORT = -5000185, // 0xFFB3B407
    /// <summary>"V Grid"</summary>
    PG_RAILING_SYSTEM_SEGMENT_V_GRID = -5000184, // 0xFFB3B408
    /// <summary>"U Grid"</summary>
    PG_RAILING_SYSTEM_SEGMENT_U_GRID = -5000183, // 0xFFB3B409
    /// <summary>"Posts"</summary>
    PG_RAILING_SYSTEM_SEGMENT_POSTS = -5000182, // 0xFFB3B40A
    /// <summary>"Pattern Remainder"</summary>
    PG_RAILING_SYSTEM_SEGMENT_PATTERN_REMAINDER = -5000181, // 0xFFB3B40B
    /// <summary>"Pattern Repeat"</summary>
    PG_RAILING_SYSTEM_SEGMENT_PATTERN_REPEAT = -5000180, // 0xFFB3B40C
    /// <summary>"Segment Pattern (default)"</summary>
    PG_RAILING_SYSTEM_FAMILY_SEGMENT_PATTERN = -5000179, // 0xFFB3B40D
    /// <summary>"Handrail 1"</summary>
    PG_RAILING_SYSTEM_FAMILY_HANDRAILS = -5000178, // 0xFFB3B40E
    /// <summary>"Top Rail"</summary>
    PG_RAILING_SYSTEM_FAMILY_TOP_RAIL = -5000177, // 0xFFB3B40F
    /// <summary>"Energy Model - Building Services"</summary>
    PG_CONCEPTUAL_ENERGY_DATA_BUILDING_SERVICES = -5000176, // 0xFFB3B410
    /// <summary>"Data"</summary>
    PG_DATA = -5000175, // 0xFFB3B411
    /// <summary>"Electrical - Circuiting"</summary>
    PG_ELECTRICAL_CIRCUITING = -5000174, // 0xFFB3B412
    /// <summary>"General"</summary>
    PG_GENERAL = -5000173, // 0xFFB3B413
    /// <summary>"Adaptive Component"</summary>
    PG_FLEXIBLE = -5000172, // 0xFFB3B414
    /// <summary>"Energy Analytical Model"</summary>
    PG_ENERGY_ANALYSIS_CONCEPTUAL_MODEL = -5000171, // 0xFFB3B415
    /// <summary>"Detailed Model"</summary>
    PG_ENERGY_ANALYSIS_DETAILED_MODEL = -5000170, // 0xFFB3B416
    /// <summary>"Essential"</summary>
    PG_ENERGY_ANALYSIS_DETAILED_AND_CONCEPTUAL_MODELS = -5000169, // 0xFFB3B417
    /// <summary>"Fittings"</summary>
    PG_FITTING = -5000168, // 0xFFB3B418
    /// <summary>"Conceptual Energy Data"</summary>
    PG_CONCEPTUAL_ENERGY_DATA = -5000167, // 0xFFB3B419
    /// <summary>"Area"</summary>
    PG_AREA = -5000166, // 0xFFB3B41A
    /// <summary>"Model Properties"</summary>
    PG_ADSK_MODEL_PROPERTIES = -5000165, // 0xFFB3B41B
    /// <summary>"V Grid"</summary>
    PG_CURTAIN_GRID_V = -5000164, // 0xFFB3B41C
    /// <summary>"U Grid"</summary>
    PG_CURTAIN_GRID_U = -5000163, // 0xFFB3B41D
    /// <summary>"Display"</summary>
    PG_DISPLAY = -5000162, // 0xFFB3B41E
    /// <summary>"Analysis Results"</summary>
    PG_ANALYSIS_RESULTS = -5000161, // 0xFFB3B41F
    /// <summary>"Slab Shape Edit"</summary>
    PG_SLAB_SHAPE_EDIT = -5000160, // 0xFFB3B420
    /// <summary>"Photometrics"</summary>
    PG_LIGHT_PHOTOMETRICS = -5000159, // 0xFFB3B421
    /// <summary>"Pattern Application"</summary>
    PG_PATTERN_APPLICATION = -5000158, // 0xFFB3B422
    /// <summary>"Green Building Properties"</summary>
    PG_GREEN_BUILDING = -5000157, // 0xFFB3B423
    /// <summary>"Profile 2"</summary>
    PG_PROFILE_2 = -5000156, // 0xFFB3B424
    /// <summary>"Profile 1"</summary>
    PG_PROFILE_1 = -5000155, // 0xFFB3B425
    /// <summary>"Profile"</summary>
    PG_PROFILE = -5000154, // 0xFFB3B426
    /// <summary>"Bottom Chords"</summary>
    PG_TRUSS_FAMILY_BOTTOM_CHORD = -5000153, // 0xFFB3B427
    /// <summary>"Top Chords"</summary>
    PG_TRUSS_FAMILY_TOP_CHORD = -5000152, // 0xFFB3B428
    /// <summary>"Diagonal Webs"</summary>
    PG_TRUSS_FAMILY_DIAG_WEB = -5000151, // 0xFFB3B429
    /// <summary>"Vertical Webs"</summary>
    PG_TRUSS_FAMILY_VERT_WEB = -5000150, // 0xFFB3B42A
    /// <summary>"Title Text"</summary>
    PG_TITLE = -5000149, // 0xFFB3B42B
    /// <summary>"Fire Protection"</summary>
    PG_FIRE_PROTECTION = -5000148, // 0xFFB3B42C
    /// <summary>"Rotation about"</summary>
    PG_ROTATION_ABOUT = -5000147, // 0xFFB3B42D
    /// <summary>"Translation in"</summary>
    PG_TRANSLATION_IN = -5000146, // 0xFFB3B42E
    /// <summary>"Analytical Model"</summary>
    PG_ANALYTICAL_MODEL = -5000145, // 0xFFB3B42F
    /// <summary>"Rebar Set"</summary>
    PG_REBAR_ARRAY = -5000144, // 0xFFB3B430
    /// <summary>"Layers"</summary>
    PG_REBAR_SYSTEM_LAYERS = -5000143, // 0xFFB3B431
    /// <summary>"Grid"</summary>
    PG_CURTAIN_GRID = -5000141, // 0xFFB3B433
    /// <summary>"Grid 2 Mullions"</summary>
    PG_CURTAIN_MULLION_2 = -5000140, // 0xFFB3B434
    /// <summary>"Horizontal Mullions"</summary>
    PG_CURTAIN_MULLION_HORIZ = -5000139, // 0xFFB3B435
    /// <summary>"Grid 1 Mullions"</summary>
    PG_CURTAIN_MULLION_1 = -5000138, // 0xFFB3B436
    /// <summary>"Vertical Mullions"</summary>
    PG_CURTAIN_MULLION_VERT = -5000137, // 0xFFB3B437
    /// <summary>"Grid 2"</summary>
    PG_CURTAIN_GRID_2 = -5000136, // 0xFFB3B438
    /// <summary>"Horizontal Grid"</summary>
    PG_CURTAIN_GRID_HORIZ = -5000135, // 0xFFB3B439
    /// <summary>"Grid 1"</summary>
    PG_CURTAIN_GRID_1 = -5000134, // 0xFFB3B43A
    /// <summary>"Vertical Grid"</summary>
    PG_CURTAIN_GRID_VERT = -5000133, // 0xFFB3B43B
    /// <summary>"IFC Parameters"</summary>
    PG_IFC = -5000131, // 0xFFB3B43D
    /// <summary>"Electrical"</summary>
    PG_ELECTRICAL = -5000130, // 0xFFB3B43E
    /// <summary>"Energy Analysis"</summary>
    PG_ENERGY_ANALYSIS = -5000129, // 0xFFB3B43F
    /// <summary>"Structural Analysis"</summary>
    PG_STRUCTURAL_ANALYSIS = -5000128, // 0xFFB3B440
    /// <summary>"Mechanical - Flow"</summary>
    PG_MECHANICAL_AIRFLOW = -5000127, // 0xFFB3B441
    /// <summary>"Mechanical - Loads"</summary>
    PG_MECHANICAL_LOADS = -5000126, // 0xFFB3B442
    /// <summary>"Electrical - Loads"</summary>
    PG_ELECTRICAL_LOADS = -5000125, // 0xFFB3B443
    /// <summary>"Electrical - Lighting"</summary>
    PG_ELECTRICAL_LIGHTING = -5000124, // 0xFFB3B444
    /// <summary>"Text"</summary>
    PG_TEXT = -5000123, // 0xFFB3B445
    /// <summary>"Camera"</summary>
    PG_VIEW_CAMERA = -5000122, // 0xFFB3B446
    /// <summary>"Extents"</summary>
    PG_VIEW_EXTENTS = -5000121, // 0xFFB3B447
    /// <summary>"Pattern"</summary>
    PG_PATTERN = -5000120, // 0xFFB3B448
    /// <summary>"Constraints"</summary>
    PG_CONSTRAINTS = -5000119, // 0xFFB3B449
    /// <summary>"Phasing"</summary>
    PG_PHASING = -5000114, // 0xFFB3B44E
    /// <summary>"Mechanical"</summary>
    PG_MECHANICAL = -5000113, // 0xFFB3B44F
    /// <summary>"Structural"</summary>
    PG_STRUCTURAL = -5000112, // 0xFFB3B450
    /// <summary>"Plumbing"</summary>
    PG_PLUMBING = -5000111, // 0xFFB3B451
    /// <summary>"Electrical Engineering"</summary>
    PG_ELECTRICAL_ENGINEERING = -5000110, // 0xFFB3B452
    /// <summary>"Stringers"</summary>
    PG_STAIR_STRINGERS = -5000109, // 0xFFB3B453
    /// <summary>"Risers"</summary>
    PG_STAIR_RISERS = -5000108, // 0xFFB3B454
    /// <summary>"Treads"</summary>
    PG_STAIR_TREADS = -5000107, // 0xFFB3B455
    /// <summary>"Underlay"</summary>
    PG_UNDERLAY = -5000106, // 0xFFB3B456
    /// <summary>"Materials and Finishes"</summary>
    PG_MATERIALS = -5000105, // 0xFFB3B457
    /// <summary>"Graphics"</summary>
    PG_GRAPHICS = -5000104, // 0xFFB3B458
    /// <summary>"Construction"</summary>
    PG_CONSTRUCTION = -5000103, // 0xFFB3B459
    /// <summary>"Dimensions"</summary>
    PG_GEOMETRY = -5000101, // 0xFFB3B45B
    /// <summary>"Identity Data"</summary>
    PG_IDENTITY_DATA = -5000100, // 0xFFB3B45C
    INVALID = -1, // 0xFFFFFFFF
}