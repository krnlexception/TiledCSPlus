namespace TiledCSPlus.Test;

public class TiledTilesetTest
{
    private TiledTileset Tileset;

    [SetUp]
    public void Setup()
    {
        Tileset = new TiledTileset("assets/tileset.tsx");
    }

    [Test]
    public void TilesetMetadata()
    {
        Tileset?.TilesetVersion.ShouldBe("1.10");
        Tileset?.TiledVersion.ShouldBe("1.10.2");
        Tileset?.Name.ShouldBe("tileset");
        Tileset?.TileWidth.ShouldBe(16);
        Tileset?.TileHeight.ShouldBe(16);
    }

    [Test]
    public void TilesetImage()
    {
        Tileset?.Image.Source.ShouldBe("tileset.png");
        Tileset?.Image.Width.ShouldBe(16);
    }

    [Test]
    public void TilesetTerrainSet()
    {
        Tileset?.TerrainSets[0].Name.ShouldBe("Unnamed Set");
        Tileset?.TerrainSets[0].Type.ShouldBe(TiledTerrainSetType.Corner);
        Tileset?.TerrainSets[0].Tile.ShouldBe(-1);
    }

    [Test]
    public void TilesetTerrainSetColor()
    {
        Tileset?.TerrainSets[0].TerrainSetColors[0].Name.ShouldBe("test123");
        Tileset?.TerrainSets[0].TerrainSetColors[0].Color.ShouldBe(Color.FromArgb(1, 2, 3));
        Tileset?.TerrainSets[0].TerrainSetColors[0].Tile.ShouldBe(-1);
        Tileset?.TerrainSets[0].TerrainSetColors[0].Probability.ShouldBe(1);
    }

    [Test]
    public void TilesetTerrainSetTile()
    {
        Tileset?.TerrainSets[0].TerrainSetTiles[0].Top.ShouldBe(0);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].TopRight.ShouldBe(1);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].Right.ShouldBe(0);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].BottomRight.ShouldBe(1);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].Bottom.ShouldBe(0);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].BottomLeft.ShouldBe(1);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].Left.ShouldBe(0);
        Tileset?.TerrainSets[0].TerrainSetTiles[0].TopLeft.ShouldBe(1);
    }
}