using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System.Linq;

[TestFixture]
public class CellTest
{
    [Test]
    public void ShouldApplyTileDataWhenSameTileID()
    {
        var cell = new Cell(0, 0);
        cell.SetTileID(1);

        var incorrectTileData = new TileData(0)
        {
            canWalk = false,
        };

        Assert.IsTrue(cell.canWalk);
        cell.ApplyTileData(incorrectTileData);
        // parameters shouldnt be changed when it has different tile id
        Assert.IsTrue(cell.canWalk);
        
        var tileData = new TileData(1)
        {
            canWalk = false,
        };

        cell.ApplyTileData(tileData);
        Assert.IsFalse(cell.canWalk);
    }
}
