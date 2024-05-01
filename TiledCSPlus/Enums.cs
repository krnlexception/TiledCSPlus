namespace TiledCSPlus
{
    /// <summary>
    /// Represents the layer type
    /// </summary>
    public enum TiledLayerType
    {
        /// <summary>
        /// Indicates that the layer is an object layer
        /// </summary>
        ObjectLayer,

        /// <summary>
        /// Indicates that the layer is a tile layer
        /// </summary>
        TileLayer,

        /// <summary>
        /// Indicates that the layer is an image layer
        /// </summary>
        ImageLayer
    }

    /// <summary>
    /// Represents property's value data type
    /// </summary>
    public enum TiledPropertyType
    {
        /// <summary>
        /// A string value
        /// </summary>
        String,

        /// <summary>
        /// A bool value
        /// </summary>
        Bool,

        /// <summary>
        /// A color value in hex format
        /// </summary>
        Color,

        /// <summary>
        /// A file path as string
        /// </summary>
        File,

        /// <summary>
        /// A float value
        /// </summary>
        Float,

        /// <summary>
        /// An int value
        /// </summary>
        Int,

        /// <summary>
        /// An object value which is the id of an object in the map
        /// </summary>
        Object
    }

    /// <summary>
    /// Represents format which tile formats are saved in
    /// </summary>
    public enum TiledTileLayerFormat
    {
        /// <summary>
        /// Base64 encoding
        /// </summary>
        Base64,

        /// <summary>
        /// Base64 encoding (Gzip compressed)
        /// </summary>
        GzipBase64,

        /// <summary>
        /// Base64 encoding (zlib compressed)
        /// </summary>
        ZlibBase64,

        /// <summary>
        /// Base64 encoding (Zstandard compressed)
        /// </summary>
        ZstdBase64,

        /// <summary>
        /// No encoding (saved as raw CSV)
        /// </summary>
        CSV
    }

    /// <summary>
    /// Represents terrain set types
    /// </summary>
    public enum TiledTerrainSetType
    {
        /// <summary>
        /// Corner type
        /// </summary>
        Corner,

        /// <summary>
        /// Edge type
        /// </summary>
        Edge,

        /// <summary>
        /// Mixed type
        /// </summary>
        Mixed
    }


    /// <summary>
    /// Represents object types in Tiled
    /// </summary>
    public enum TiledObjectType
    {
        /// <summary>
        /// Point object type
        /// </summary>
        Point,

        /// <summary>
        /// Eclipse object type
        /// </summary>
        Eclipse,

        /// <summary>
        /// Polygon object type
        /// </summary>
        Polygon,

        /// <summary>
        /// Polyline object type (used when polygon object is not closed)
        /// </summary>
        Polyline,

        /// <summary>
        /// Tile object type
        /// </summary>
        Tile,

        /// <summary>
        /// Rectangular object type
        /// </summary>
        Rectangular
    }
}