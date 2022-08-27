using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [SerializeField] private AudioClip collectSound;

    [SerializeField] private AudioSource _audioSource;

    public Row[] rows;

    public Tile[,] Tiles { get; private set; }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.25f;

    public Slider memorySlider;

    public float memorySliderModifier = 2;


    private void Awake() => Instance = this;

    private bool gameStarted;


    private void Start()
    {
        CreateBoard();
        gameStarted = true;
    }

    private void Update()
    {
        memorySliderModifier += Time.deltaTime;
        memorySlider.value += Time.deltaTime * memorySliderModifier;
        if (memorySlider.value == 1000) Application.Quit();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateBoard();
        }
    }

    public void CreateBoard()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                Tiles[x, y] = tile;
            }
        }
        if (gameStarted)
        {
            memorySlider.value += 100;
            ScoreCounter.Instance.Score -= 100;
        }
    }

    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if(_selection.Count > 0)
            {
                if(Array.IndexOf(_selection[0].Neighbors, tile) != -1)
                {
                    _selection.Add(tile);
                    _selection[1].Shake();
                }
                else
                {
                    _selection[0].StopShake();
                    _selection.Clear();
                }
            }
            else
            {
                _selection.Add(tile);
                _selection[0].Shake();
            }
            
        } 


        if (_selection.Count < 2) return;

        Debug.Log($"Selected tiles at: ({_selection[0].x},{_selection[0].y}) and ({_selection[1].x},{_selection[1].y})");

        await Swap(_selection[0], _selection[1]);
        

        if (CanPop())
        {
            Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
        }

        _selection.Clear();
    }


    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;

        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
            .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play() // Sequence
            .AsyncWaitForCompletion(); // Task

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;

        tile1.StopShake();
        tile2.StopShake();
    }

    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) return true;

        return false;
    }

    private async void Pop()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2) continue;

                var deflateSequence = DOTween.Sequence();


                foreach (var connectedTile in connectedTiles) deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));

                _audioSource.PlayOneShot(collectSound);

                var pointGain = tile.Item.value * connectedTiles.Count();

                ScoreCounter.Instance.Score += pointGain;

                memorySlider.value -= pointGain;
                
                await deflateSequence.Play()
                                     .AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();

                foreach(var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                    inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }

                await inflateSequence.Play().
                                      AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }
    }
}
