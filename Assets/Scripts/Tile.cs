using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    private Item _item;

    public Tween shakeTween;

    public Item Item
    {
        get { return _item; }

        set 
        {
            if (_item == value) return;
        
            _item = value;

            icon.sprite = _item.sprite;

            text.text = _item.appName;
        }
    }

    public Image icon;

    public Button button;

    public TextMeshProUGUI text;

    public Tile Left => x > 0 ? Board.Instance.Tiles[x - 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.Tiles[x, y - 1] : null;
    public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.Tiles[x + 1, y] : null;
    public Tile Bottom => y < Board.Instance.Height - 1 ? Board.Instance.Tiles[x, y + 1] : null;

    public Tile[] Neighbors => new[]
    {
        Left,
        Top,
        Right,
        Bottom
    };

    private void Start() => button.onClick.AddListener(() => Board.Instance.Select(this));

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, };

        if(exclude == null)
        {
            exclude = new List<Tile> { this, };
        }
        else
        {
            exclude.Add(this);
        }

        foreach(var neighbor in Neighbors)
        {
            if (neighbor == null || exclude.Contains(neighbor) || neighbor.Item != Item) continue;
            
            result.AddRange(neighbor.GetConnectedTiles(exclude));
        }

        return result;
    }

    static class ShakeAnimProperties
    {
        static public float Duration { get { return 0.5f; } }
        static public Vector3 Punch { get { return new Vector3(0, 0, 10); } }
        static public int Vibrato { get { return 5; } }
        static public float Elasticity { get { return 0f; } }
    }

    public void Shake()
    {
        shakeTween = transform.DOPunchRotation(ShakeAnimProperties.Punch, ShakeAnimProperties.Duration, ShakeAnimProperties.Vibrato, ShakeAnimProperties.Elasticity).SetAutoKill(false);
        shakeTween.OnComplete(() => shakeTween.Restart());
        shakeTween.Play();
    }

    public void StopShake()
    {
        shakeTween.OnComplete(null);
        shakeTween.SetAutoKill(true);
    }
}
