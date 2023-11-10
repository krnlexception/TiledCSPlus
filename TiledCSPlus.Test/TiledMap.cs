namespace TiledCSPlus.Test;

public class TiledMapTest
{
    private TiledMap? TiledMap;
    private TiledMap? TiledMap19;
    private Dictionary<int, TiledTileset>? Tilesets;

    [SetUp]
    public void Setup()
    {
        TiledMap = new TiledMap("assets/tilemap1.10.tmx");
        TiledMap19 = new("assets/tilemap1.9.tmx");
        Tilesets = TiledMap.GetTiledTilesets("assets/");
    }

    [Test]
    public void MapMetadata()
    {
        TiledMap?.TiledVersion.ShouldBe("1.10.2");
        TiledMap?.MapVersion.ShouldBe("1.10");
        TiledMap?.Width.ShouldBe(10);
        TiledMap?.Height.ShouldBe(10);
        TiledMap?.TileHeight.ShouldBe(16);
        TiledMap?.TileWidth.ShouldBe(16);
        TiledMap?.Orientation.ShouldBe("orthogonal");
        TiledMap?.BackgroundColor.ShouldBe(new Color(1, 2, 3, 4));
    }

    [Test]
    public void Tileset()
    {
        TiledMap?.Tilesets[0].FirstGid.ShouldBe(1);
        TiledMap?.Tilesets[0].Source.ShouldBe("tileset.tsx");
        TiledMap?.Tilesets[0].IsTilesetEmbedded.ShouldBe(false);
        TiledMap?.Tilesets[1].FirstGid.ShouldBe(2);
        TiledMap?.Tilesets[1].Source.ShouldBe(null);
        TiledMap?.Tilesets[1].IsTilesetEmbedded.ShouldBe(true);

        Tilesets?[1].Image.Source.ShouldBe("tileset.png");
        Tilesets?[1].TilesetVersion.ShouldBe("1.8");
        TiledMap?.EmbeddedTilesets[2].Name.ShouldBe("tileset-embedded110");
    }

    [Test]
    public void Layers()
    {
        TiledMap?.Layers[0].TintColor.ShouldBe(new Color(255, 254, 253, 252));
        TiledMap?.Layers[2].Name.ShouldBe("Image Layer 1");
        TiledMap?.Layers[1].Objects[0].Class.ShouldBe("test110");
    }

    [Test]
    public void CompabilityWith19()
    {
        TiledMap19?.MapVersion.ShouldBe("1.9");
        TiledMap19?.Layers[1].Objects[0].Class.ShouldBe("test19");
    }
}