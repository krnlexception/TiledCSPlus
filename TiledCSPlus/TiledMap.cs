using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using TiledCSPlus;

namespace TiledCSPlus
{
    /// <summary>
    /// Represents a Tiled map
    /// </summary>
    public class TiledMap
    {
        const uint FLIPPED_HORIZONTALLY_FLAG = 0b10000000000000000000000000000000;
        const uint FLIPPED_VERTICALLY_FLAG = 0b01000000000000000000000000000000;
        const uint FLIPPED_DIAGONALLY_FLAG = 0b00100000000000000000000000000000;

        /// <summary>
        /// How many times we shift the FLIPPED flags to the right in order to store it in a byte.
        /// For example: 0b10100000000000000000000000000000 >> SHIFT_FLIP_FLAG_TO_BYTE = 0b00000101
        /// </summary>
        const int SHIFT_FLIP_FLAG_TO_BYTE = 29;

        /// <summary>
        /// Returns the Tiled version used to create this map
        /// </summary>
        public string TiledVersion { get; internal set; }

        /// <summary>
        /// Returns an array of properties defined in the map
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// Returns an array of tileset definitions in the map
        /// </summary>
        public TiledMapTileset[] Tilesets { get; internal set; }

        /// <summary>
        /// Returns an array of layers or null if none were defined
        /// </summary>
        public TiledLayer[] Layers { get; internal set; }

        /// <summary>
        /// Returns an array of groups or null if none were defined
        /// </summary>
        public TiledGroup[] Groups { get; internal set; }

        /// <summary>
        /// Returns the defined map orientation as a string
        /// </summary>
        public string Orientation { get; internal set; }

        /// <summary>
        /// Returns the render order as a string
        /// </summary>
        public string RenderOrder { get; internal set; }

        /// <summary>
        /// The amount of horizontal tiles
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// The amount of vertical tiles
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// The tile width in pixels
        /// </summary>
        public int TileWidth { get; internal set; }

        /// <summary>
        /// The tile height in pixels
        /// </summary>
        public int TileHeight { get; internal set; }

        /// <summary>
        /// The parallax origin x
        /// </summary>
        public float ParallaxOriginX { get; internal set; }

        /// <summary>
        /// The parallax origin y
        /// </summary>
        public float ParallaxOriginY { get; internal set; }

        /// <summary>
        /// Returns true if the map is configured as infinite
        /// </summary>
        public bool Infinite { get; internal set; }

        /// <summary>
        /// Returns the defined map background color as a TiledColor
        /// </summary>
        public Color BackgroundColor { get; internal set; }

        /// <summary>
        /// Returns tilesets embedded in the map
        /// </summary>
        public Dictionary<int, TiledTileset> EmbeddedTilesets { get; internal set; }

        /// <summary>
        /// Returns an empty instance of TiledMap
        /// </summary>
        public TiledMap()
        {
        }

        /// <summary>
        /// Loads a Tiled map in TMX format and parses it
        /// </summary>
        /// <param name="path">The path to the tmx file</param>
        /// <exception cref="TiledException">Thrown when the map could not be loaded or is not in a correct format</exception>
        public TiledMap(string path)
        {
            // Check the file
            if (!File.Exists(path))
            {
                throw new TiledException($"{path} not found");
            }

            var content = File.ReadAllText(path);

            if (path.EndsWith(".tmx"))
            {
                ParseXml(content);
            }
            else
            {
                throw new TiledException("Unsupported file format");
            }
        }

        /// <summary>
        /// Loads a Tiled map in TMX format and parses it
        /// </summary>
        /// <param name="stream">Stream of opened tmx file</param>
        /// <exception cref="TiledException">Thrown when the map could not be loaded</exception>
        public TiledMap(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var content = streamReader.ReadToEnd();
            ParseXml(content);
        }

        /// <summary>
        /// Can be used to parse the content of a TMX map manually instead of loading it using the constructor
        /// </summary>
        /// <param name="xml">The tmx file content as string</param>
        /// <exception cref="TiledException"></exception>
        public void ParseXml(string xml)
        {
            try
            {
                // Load the xml document
                var document = new XmlDocument();
                document.LoadXml(xml);

                var nodeMap = document.SelectSingleNode("map");
                var nodesProperty = nodeMap.SelectNodes("properties/property");
                var nodesLayer = nodeMap.SelectNodes("layer | objectgroup | imagelayer");
                var nodesTileset = nodeMap.SelectNodes("tileset");
                var nodesGroup = nodeMap.SelectNodes("group");
                var attrParallaxOriginX = nodeMap.Attributes["parallaxoriginx"];
                var attrParallaxOriginY = nodeMap.Attributes["parallaxoriginy"];
                var attrBackgroundColor = nodeMap.Attributes["backgroundcolor"];

                TiledVersion = nodeMap.Attributes["tiledversion"].Value;
                Orientation = nodeMap.Attributes["orientation"].Value;
                RenderOrder = nodeMap.Attributes["renderorder"].Value;
                Infinite = nodeMap.Attributes["infinite"].Value == "1";
                EmbeddedTilesets = new Dictionary<int, TiledTileset>();

                Width = int.Parse(nodeMap.Attributes["width"].Value);
                Height = int.Parse(nodeMap.Attributes["height"].Value);
                TileWidth = int.Parse(nodeMap.Attributes["tilewidth"].Value);
                TileHeight = int.Parse(nodeMap.Attributes["tileheight"].Value);


                if (nodesProperty != null) Properties = ParseProperties(nodesProperty);
                if (nodesTileset != null) Tilesets = ParseTilesets(nodesTileset);
                if (nodesLayer != null) Layers = ParseLayers(nodesLayer);
                if (nodesGroup != null) Groups = ParseGroups(nodesGroup);
                if (attrParallaxOriginX != null) ParallaxOriginX = float.Parse(attrParallaxOriginX.Value, CultureInfo.InvariantCulture);
                if (attrParallaxOriginY != null) ParallaxOriginY = float.Parse(attrParallaxOriginY.Value, CultureInfo.InvariantCulture);
                if (attrBackgroundColor != null) BackgroundColor = ParseColor(attrBackgroundColor.Value);
            }
            catch (Exception ex)
            {
                throw new TiledException("An error occurred while trying to parse the Tiled map file", ex);
            }
        }

        private TiledProperty[] ParseProperties(XmlNodeList nodeList)
        {
            var result = new List<TiledProperty>();

            foreach (XmlNode node in nodeList)
            {
                var attrType = node.Attributes["type"];
                
                var property = new TiledProperty();
                property.Name = node.Attributes["name"].Value;
                property.Value = node.Attributes["value"]?.Value;
                property.Type = TiledPropertyType.String;

                if (attrType != null)
                {
                    if (attrType.Value == "bool") property.Type = TiledPropertyType.Bool;
                    if (attrType.Value == "color") property.Type = TiledPropertyType.Color;
                    if (attrType.Value == "file") property.Type = TiledPropertyType.File;
                    if (attrType.Value == "float") property.Type = TiledPropertyType.Float;
                    if (attrType.Value == "int") property.Type = TiledPropertyType.Int;
                    if (attrType.Value == "object") property.Type = TiledPropertyType.Object;
                }

                if (property.Value == null)
                {
                    property.Value = node.InnerText;
                }

                result.Add(property);
            }

            return result.ToArray();
        }

        private TiledMapTileset[] ParseTilesets(XmlNodeList nodeList)
        {
            var result = new List<TiledMapTileset>();

            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes["source"] == null)
                {
                    //tilemap is an embedded tilemap
                    TiledTileset tileset = new TiledTileset();
                    tileset.ParseXml(node.OuterXml);
                    int firstgid = int.Parse(node.Attributes["firstgid"].Value);
                    EmbeddedTilesets.Add(firstgid, tileset);
                    var maptileset = new TiledMapTileset();
                    maptileset.FirstGid = int.Parse(node.Attributes["firstgid"].Value);
                    maptileset.IsTilesetEmbedded = true;
                    result.Add(maptileset);
                }
                else
                {
                    var tileset = new TiledMapTileset();
                    tileset.FirstGid = int.Parse(node.Attributes["firstgid"].Value);
                    tileset.Source = node.Attributes["source"]?.Value;
                    tileset.IsTilesetEmbedded = false;
                    result.Add(tileset);
                }
            }

            return result.ToArray();
        }

        private TiledGroup[] ParseGroups(XmlNodeList nodeListGroups)
        {
            var result = new List<TiledGroup>();

            foreach (XmlNode node in nodeListGroups)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodesGroup = node.SelectNodes("group");
                var nodesLayer = node.SelectNodes("layer | objectgroup | imagelayer");
                var attrVisible = node.Attributes["visible"];
                var attrLocked = node.Attributes["locked"];

                var tiledGroup = new TiledGroup();
                tiledGroup.Id = int.Parse(node.Attributes["id"].Value);
                tiledGroup.Name = node.Attributes["name"].Value;

                if (attrVisible != null) tiledGroup.Visible = attrVisible.Value == "1";
                if (attrLocked != null) tiledGroup.Locked = attrLocked.Value == "1";
                if (nodesProperty != null) tiledGroup.Properties = ParseProperties(nodesProperty);
                if (nodesGroup != null) tiledGroup.Groups = ParseGroups(nodesGroup);
                if (nodesLayer != null) tiledGroup.Layers = ParseLayers(nodesLayer);

                result.Add(tiledGroup);
            }

            return result.ToArray();
        }

        private TiledLayer[] ParseLayers(XmlNodeList nodesLayer)
        {
            var result = new List<TiledLayer>();
            foreach (XmlNode node in nodesLayer)
            {
                switch (node.Name)
                {
                    case "layer":
                        result.Add(ParseLayer(node, TiledLayerType.TileLayer));
                        break;
                    case "objectgroup":
                        result.Add(ParseLayer(node, TiledLayerType.ObjectLayer));
                        break;
                    case "imagelayer":
                        result.Add(ParseLayer(node, TiledLayerType.ImageLayer));
                        break;
                    default:
                        throw new TiledException($"Unknown layer type: {node.Name}");
                }
            }

            return result.ToArray();
        }

        private TiledLayer ParseLayer(XmlNode node, TiledLayerType type)
        {
            var nodesProperty = node.SelectNodes("properties/property");
            var attrVisible = node.Attributes["visible"];
            var attrLocked = node.Attributes["locked"];
            var attrTint = node.Attributes["tintcolor"];
            var attrOffsetX = node.Attributes["offsetx"];
            var attrOffsetY = node.Attributes["offsety"];
            var attrParallaxX = node.Attributes["parallaxx"];
            var attrParallaxY = node.Attributes["parallaxy"];
            var attrOpacity = node.Attributes["opacity"];
            var attrClass = node.Attributes["class"];
            var attrWidth = node.Attributes["width"];
            var attrHeight = node.Attributes["height"];

            var tiledLayer = new TiledLayer();
            tiledLayer.Id = int.Parse(node.Attributes["id"].Value);
            tiledLayer.Type = type;
            tiledLayer.Name = node.Attributes["name"].Value;
            tiledLayer.Visible = true;
            tiledLayer.Opacity = 1.0f;
            tiledLayer.Parrallax = new Vector2(1.0f, 1.0f);

            if (attrWidth != null || attrWidth != null) tiledLayer.Size = new Size(0, 0);
            if (attrWidth != null) tiledLayer.Size.Width = int.Parse(attrWidth.Value);
            if (attrHeight != null) tiledLayer.Size.Height = int.Parse(attrHeight.Value);
            if (attrVisible != null) tiledLayer.Visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.Locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.TintColor = ParseColor(attrTint.Value);
            if (attrClass != null) tiledLayer.Class = attrClass.Value;
            if (attrOpacity != null) tiledLayer.Opacity = float.Parse(attrOpacity.Value, CultureInfo.InvariantCulture);
            if (attrOffsetX != null || attrOffsetY != null) tiledLayer.Offset = new Vector2(0, 0);
            if (attrOffsetX != null) tiledLayer.Offset.X = float.Parse(attrOffsetX.Value, CultureInfo.InvariantCulture);
            if (attrOffsetY != null) tiledLayer.Offset.Y = float.Parse(attrOffsetY.Value, CultureInfo.InvariantCulture);
            if (attrParallaxX != null) tiledLayer.Parrallax.X = float.Parse(attrParallaxX.Value, CultureInfo.InvariantCulture);
            if (attrParallaxY != null) tiledLayer.Parrallax.Y = float.Parse(attrParallaxY.Value, CultureInfo.InvariantCulture);
            if (nodesProperty != null) tiledLayer.Properties = ParseProperties(nodesProperty);

            if (type == TiledLayerType.TileLayer)
            {
                var nodeData = node.SelectSingleNode("data");
                
                ParseTileLayerData(nodeData, ref tiledLayer);
            }

            if (type == TiledLayerType.ObjectLayer)
            {
                var nodesObject = node.SelectNodes("object");
                
                tiledLayer.Objects = ParseObjects(nodesObject);
            }

            if (type == TiledLayerType.ImageLayer)
            {
                var nodeImage = node.SelectSingleNode("image");
                
                if (nodeImage != null) tiledLayer.Image = ParseImage(nodeImage);
            }

            return tiledLayer;
        }

        private void ParseTileLayerData(XmlNode nodeData, ref TiledLayer tiledLayer)
        {
            var encoding = nodeData.Attributes["encoding"].Value;
            var compression = nodeData.Attributes["compression"]?.Value;

            if (encoding != "csv" && encoding != "base64")
            {
                throw new TiledException("Only CSV and Base64 encodings are currently supported");
            }

            if (Infinite)
            {
                var nodesChunk = nodeData.SelectNodes("chunk");
                var chunks = new List<TiledChunk>();

                foreach (XmlNode nodeChunk in nodesChunk)
                {
                    var chunk = new TiledChunk();
                    chunk.Position = new Vector2(int.Parse(nodeChunk.Attributes["x"].Value),
                        int.Parse(nodeChunk.Attributes["y"].Value));
                    chunk.Size = new Size(int.Parse(nodeChunk.Attributes["width"].Value), int.Parse(nodeChunk.Attributes["height"].Value));

                    int[] chunkData = new int[]{};
                    byte[] chunkDataRotationFlags = new byte[] { };
                    if (encoding == "csv") ParseTileLayerDataAsCSV(nodeChunk.InnerText, out chunkData, out chunkDataRotationFlags);
                    if (encoding == "base64") ParseTileLayerDataAsBase64(nodeChunk.InnerText, compression, out chunkData, out chunkDataRotationFlags);
                    chunk.Data = chunkData;
                    chunk.DataRotationFlags = chunkDataRotationFlags;

                    chunks.Add(chunk);
                }

                tiledLayer.Chunks = chunks.ToArray();
            }
            else
            {
                int[] chunkData = new int[]{};
                byte[] chunkDataRotationFlags = new byte[] { };
                if (encoding == "csv") ParseTileLayerDataAsCSV(nodeData.InnerText, out chunkData, out chunkDataRotationFlags);
                if (encoding == "base64") ParseTileLayerDataAsBase64(nodeData.InnerText, compression, out chunkData, out chunkDataRotationFlags);
                tiledLayer.Data = chunkData;
                tiledLayer.DataRotationFlags = chunkDataRotationFlags;
            }
        }

        private void ParseTileLayerDataAsBase64(string input, string compression, out int[] data, out byte[] dataRotationFlags)
        {
            using (var base64DataStream = new MemoryStream(Convert.FromBase64String(input)))
            {
                if (compression == null)
                {
                    // Parse the decoded bytes and update the inner data as well as the data rotation flags
                    var rawBytes = new byte[4];
                    data = new int[base64DataStream.Length];
                    dataRotationFlags = new byte[base64DataStream.Length];

                    for (var i = 0; i < base64DataStream.Length; i++)
                    {
                        base64DataStream.Read(rawBytes, 0, rawBytes.Length);
                        var rawID = BitConverter.ToUInt32(rawBytes, 0);
                        var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
                        var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
                        var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
                        dataRotationFlags[i] = (byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE);

                        // assign data to rawID with the rotation flags cleared
                        data[i] = (int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG));
                    }
                }
                else if (compression == "zlib")
                {
                    // .NET doesn't play well with the headered zlib data that Tiled produces,
                    // so we have to manually skip the 2-byte header to get what DeflateStream's looking for
                    // Should an external library be used instead of this hack?
                    base64DataStream.ReadByte();
                    base64DataStream.ReadByte();

                    using (var decompressionStream = new DeflateStream(base64DataStream, CompressionMode.Decompress))
                    {
                        // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
                        var decompressedDataBuffer = new byte[4]; // size of each tile
                        var dataRotationFlagsList = new List<byte>();
                        var layerDataList = new List<int>();

                        while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) == decompressedDataBuffer.Length)
                        {
                            var rawID = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                            var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
                            var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
                            var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
                            dataRotationFlagsList.Add((byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE));

                            // assign data to rawID with the rotation flags cleared
                            layerDataList.Add((int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG)));
                        }

                        data = layerDataList.ToArray();
                        dataRotationFlags = dataRotationFlagsList.ToArray();
                    }
                }
                else if (compression == "gzip")
                {
                    using (var decompressionStream = new GZipStream(base64DataStream, CompressionMode.Decompress))
                    {
                        // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
                        var decompressedDataBuffer = new byte[4]; // size of each tile
                        var dataRotationFlagsList = new List<byte>();
                        var layerDataList = new List<int>();

                        while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) == decompressedDataBuffer.Length)
                        {
                            var rawID = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                            var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
                            var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
                            var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));

                            dataRotationFlagsList.Add((byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE));

                            // assign data to rawID with the rotation flags cleared
                            layerDataList.Add((int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG)));
                        }

                        data = layerDataList.ToArray();
                        dataRotationFlags = dataRotationFlagsList.ToArray();
                    }
                }
                else
                {
                    throw new TiledException("Zstandard compression is currently not supported");
                }
            }
        }

        private void ParseTileLayerDataAsCSV(string input, out int[] data, out byte[] dataRotationFlags)
        {
            var csvs = input.Split(',');

            data = new int[csvs.Length];
            dataRotationFlags = new byte[csvs.Length];

            // Parse the comma separated csv string and update the inner data as well as the data rotation flags
            for (var i = 0; i < csvs.Length; i++)
            {
                var rawID = uint.Parse(csvs[i]);
                var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
                var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
                var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
                dataRotationFlags[i] = (byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE);

                // assign data to rawID with the rotation flags cleared
                data[i] = (int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG));
            }
        }

        private TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage();
            tiledImage.Source = node.Attributes["source"].Value;
            tiledImage.Size = new Size(int.Parse(node.Attributes["width"].Value),
                int.Parse(node.Attributes["height"].Value));

            return tiledImage;
        }

        private TiledObject[] ParseObjects(XmlNodeList nodeList)
        {
            var result = new List<TiledObject>();

            foreach (XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodePolygon = node.SelectSingleNode("polygon");
                var nodePoint = node.SelectSingleNode("point");
                var nodeEllipse = node.SelectSingleNode("ellipse");
                var attrGid = node.Attributes["gid"];

                var obj = new TiledObject();
                obj.Id = int.Parse(node.Attributes["id"].Value);
                obj.Name = node.Attributes["name"]?.Value;
                obj.Class = node.Attributes["class"]?.Value;
                obj.Type = node.Attributes["type"]?.Value;
                obj.Position = new Vector2(float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture),
                    float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture));

                if (attrGid != null)
                {
                    ParseObjectGid(ref obj, attrGid.Value);
                }

                if (nodesProperty != null)
                {
                    obj.Properties = ParseProperties(nodesProperty);
                }

                if (nodePolygon != null)
                {
                    var points = nodePolygon.Attributes["points"].Value;
                    var vertices = points.Split(' ');

                    var polygon = new TiledPolygon();
                    polygon.Points = new Vector2[vertices.Length];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        polygon.Points[i] =
                            new Vector2(float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture),
                                float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture));
                    }

                    obj.Polygon = polygon;
                }

                if (nodeEllipse != null)
                {
                    obj.Ellipse = new TiledEllipse();
                }

                if (nodePoint != null)
                {
                    obj.Point = new TiledPoint();
                }

                if (node.Attributes["width"] != null || node.Attributes["height"] != null) obj.Size = new Size(0, 0);
                if (node.Attributes["width"] != null)
                {
                    obj.Size.Width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);
                }

                if (node.Attributes["height"] != null)
                {
                    obj.Size.Height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);
                }

                if (node.Attributes["rotation"] != null)
                {
                    obj.Rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);
                }

                result.Add(obj);
            }

            return result.ToArray();
        }

        private void ParseObjectGid(ref TiledObject tiledObject, String gid)
        {
            var rawID = uint.Parse(gid);
            var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
            var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
            var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
            
            tiledObject.DataRotationFlag = (byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE);
            tiledObject.Gid = (int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG));
        }

        private Color ParseColor(string hexColor)
        {
            hexColor = hexColor.Substring(1);
            List<byte> color = new List<byte>();
            for (int i = 0; i < hexColor.Length - 1; i += 2)
            {
                color.Add(Convert.ToByte(hexColor.Substring(i, 2), 16));
            }

            return color.Count == 3 ? new Color(color[0], color[1], color[2]) : new Color(color[1], color[2], color[3], color[0]);;
        }

        /* HELPER METHODS */
        /// <summary>
        /// Locates the right TiledMapTileset object for you within the Tilesets array
        /// </summary>
        /// <param name="gid">A value from the TiledLayer.data array</param>
        /// <returns>An element within the Tilesets array or null if no match was found</returns>
        public TiledMapTileset GetTiledMapTileset(int gid)
        {
            if (Tilesets == null)
            {
                return null;
            }

            for (var i = 0; i < Tilesets.Length; i++)
            {
                if (i < Tilesets.Length - 1)
                {
                    int gid1 = Tilesets[i + 0].FirstGid;
                    int gid2 = Tilesets[i + 1].FirstGid;

                    if (gid >= gid1 && gid < gid2)
                    {
                        return Tilesets[i];
                    }
                }
                else
                {
                    return Tilesets[i];
                }
            }

            return new TiledMapTileset();
        }

        /// <summary>
        /// Loads external tilesets and matches them to firstGids from elements within the Tilesets array
        /// </summary>
        /// <param name="src">The folder where the TiledMap file is located</param>
        /// <returns>A dictionary where the key represents the firstGid of the associated TiledMapTileset and the value the TiledTileset object</returns>
        public Dictionary<int, TiledTileset> GetTiledTilesets(string src)
        {
            var tilesets = new Dictionary<int, TiledTileset>();
            var info = new FileInfo(src);
            var srcFolder = info.Directory;

            if (Tilesets == null)
            {
                return tilesets;
            }

            foreach (var mapTileset in Tilesets)
            {
                var path = $"{srcFolder}/{mapTileset.Source}";

                if (mapTileset.Source == null)
                {
                    continue;
                }

                if (File.Exists(path))
                {
                    tilesets.Add(mapTileset.FirstGid, new TiledTileset(path));
                }
                else
                {
                    throw new TiledException("Cannot locate tileset '" + path + "'. Please make sure the source folder is correct and it ends with a slash.");
                }
            }

            return tilesets;
        }

        /// <summary>
        /// Locates a specific TiledTile object
        /// </summary>
        /// <param name="mapTileset">An element within the Tilesets array</param>
        /// <param name="tileset">An instance of the TiledTileset class</param>
        /// <param name="gid">An element from within a TiledLayer.data array</param>
        /// <returns>An entry of the TiledTileset.tiles array or null if none of the tile id's matches the gid</returns>
        /// <remarks>Tip: Use the GetTiledMapTileset and GetTiledTilesets methods for retrieving the correct TiledMapTileset and TiledTileset objects</remarks>
        public TiledTile GetTiledTile(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            foreach (var tile in tileset.Tiles)
            {
                if (tile.Id == gid - mapTileset.FirstGid)
                {
                    return tile;
                }
            }

            return null;
        }

        /// <summary>
        /// This method can be used to figure out the source rect on a Tileset image for rendering tiles.
        /// </summary>
        /// <param name="mapTileset"></param>
        /// <param name="tileset"></param>
        /// <param name="gid"></param>
        /// <returns>An instance of the class TiledSourceRect that represents a rectangle. Returns null if the provided gid was not found within the tileset.</returns>
        public TiledSourceRect GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            var tileHor = 0;
            var tileVert = 0;

            for (var i = 0; i < tileset.TileCount; i++)
            {
                if (i == gid - mapTileset.FirstGid)
                {
                    var result = new TiledSourceRect();
                    result.Position = new Vector2(tileHor * tileset.TileWidth, tileVert * tileset.TileHeight);
                    result.Size = new Size(tileset.TileWidth, tileset.TileHeight);

                    return result;
                }

                // Update x and y position
                tileHor++;

                if (tileHor == tileset.Image.Size.Width / tileset.TileWidth)
                {
                    tileHor = 0;
                    tileVert++;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
            {
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");
            }
            
            return IsTileFlippedHorizontal(layer, tileHor + (tileVert * (int)layer.Size.Width));
        }

        /// <summary>
        /// Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledLayer layer, int dataIndex)
        {
            return (layer.DataRotationFlags[dataIndex] & (FLIPPED_HORIZONTALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
        }
        
        /// <summary>
        /// Checks is a tile linked to an object is flipped horizontally
        /// </summary>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0)
            {
                throw new TiledException("Tiled object not linked to a tile");
            }
            
            return (tiledObject.DataRotationFlag & (FLIPPED_HORIZONTALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
        }

        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
            {
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");
            }
            
            return IsTileFlippedVertical(layer, tileHor + (tileVert * (int)layer.Size.Width));
        }

        /// <summary>
        /// Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledLayer layer, int dataIndex)
        {
            return (layer.DataRotationFlags[dataIndex] & (FLIPPED_VERTICALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
        }
        
        /// <summary>
        /// Checks is a tile linked to an object is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedVertical(TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0)
            {
                throw new TiledException("Tiled object not linked to a tile");
            }
            
            return (tiledObject.DataRotationFlag & (FLIPPED_VERTICALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
        }

        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="tileHor">The tile's horizontal position</param>
        /// <param name="tileVert">The tile's vertical position</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledLayer layer, int tileHor, int tileVert)
        {
            if (layer.Type != TiledLayerType.TileLayer)
            {
                throw new TiledException("Retrieving tile flipped state for a tile does not work for non-tile layers");
            }
            
            return IsTileFlippedDiagonal(layer, tileHor + (tileVert * (int)layer.Size.Width));
        }

        /// <summary>
        /// Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledLayer layer, int dataIndex)
        {
            return (layer.DataRotationFlags[dataIndex] & (FLIPPED_DIAGONALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
        }
        
        /// <summary>
        /// Checks is a tile linked to an object is flipped diagonally
        /// </summary>
        /// <param name="tiledObject">The tiled object</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledObject tiledObject)
        {
            if (tiledObject.Gid == 0)
            {
                throw new TiledException("Tiled object not linked to a tile");
            }
            
            return (tiledObject.DataRotationFlag & (FLIPPED_DIAGONALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
        }
    }
}
