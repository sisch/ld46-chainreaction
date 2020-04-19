using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Rod : MonoBehaviour, IPointerClickHandler
{
    public Player ownedBy;

    public struct GridCoordinate
    {
        public int x;
        public int y;
    }
    public List<Neutron> neutrons;
    public int maxNumberOfNeutrons;
    public Transform target;
    public GridCoordinate gridCoordinate;
    public float ownershipDelay = 0.2f;
    private MeshRenderer _meshRenderer;

    private List<GridCoordinate> AdjacentCoords;
    // Start is called before the first frame update
    private void Awake()
    {
        neutrons = new List<Neutron>();
        _meshRenderer = GetComponent<MeshRenderer>();
        AdjacentCoords = new List<GridCoordinate>();
        gridCoordinate.x = (int) transform.position.x;
        gridCoordinate.y = (int) transform.position.z;
    }

    void Start()
    {

        GameManager.Instance.RegisterRod(this);
        CalculateAdjacent();
    }

    private void CalculateAdjacent()
    {
        int width = GameManager.Instance.gameWidth;
        int height = GameManager.Instance.gameHeight;
        int[] xs = {-1, 0, 1, 0};
        int[] ys = {0, 1, 0, -1};
        for (int i = 0; i < 4; i++)
        {
            if (gridCoordinate.x + xs[i] >= 0 &&
                gridCoordinate.x + xs[i] < width &&
                gridCoordinate.y + ys[i] >= 0 &&
                gridCoordinate.y + ys[i] < height)
            {
                AdjacentCoords.Add(new GridCoordinate()
                {
                    x = gridCoordinate.x + xs[i],
                    y = gridCoordinate.y + ys[i]
                });
            }
        }
        maxNumberOfNeutrons = AdjacentCoords.Count;
        Debug.Log($"Rod {gridCoordinate} has {maxNumberOfNeutrons} neighbours");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{gridCoordinate.x}/{gridCoordinate.y}: {neutrons.Count}");
        Player currentPlayer = GameManager.Instance.currentPlayer;
        if (currentPlayer.isActive && (currentPlayer == ownedBy || ownedBy == null))
        {
            currentPlayer.newNeutron.transform.position = Random.onUnitSphere + target.position + 4 * Vector3.up;
            currentPlayer.newNeutron.SetTarget(target);
            AddNeutron(currentPlayer.newNeutron);
            StartCoroutine("SetOwner", currentPlayer);
            currentPlayer.isActive = false;
            currentPlayer.newNeutron = null;
            GameManager.Instance.InvokeNextPlayer();
        }
    }

    public void SetOwner(Player player)
    {
        ownedBy = player;
        StartCoroutine(_SetOwner(player));
    }

    public IEnumerator _SetOwner(Player player)
    {
        yield return new WaitForSeconds(ownershipDelay);
        foreach (var neutron in neutrons)
        {
            neutron.SetOwner(player);
        }
        _meshRenderer.material = player.playerMat;        
    }
    public void AddNeutron(Neutron neutron)
    {
        if (neutrons.Count + 1>maxNumberOfNeutrons)
        {
            Neutron[] ns = neutrons.ToArray();
            for (var index = 0; index < AdjacentCoords.Count; index++)
            {
                var neutron1 = ns[index];
                var targetRod = GameManager.Instance.GetRodAt(AdjacentCoords[index]);
                MoveNeutron(neutron1, targetRod);
            }
        }
        neutrons.Add(neutron);
    }

    public IEnumerator AddNeutronWithDelay(Neutron neutron)
    {
        yield return new WaitForSeconds(Settings.useDelay?GameManager.Instance.NeutronDelay:0.01f);

        AddNeutron(neutron);
        yield return null;
    }
    void MoveNeutron(Neutron neutron, Rod target)
    {
        neutron.SetTarget(target.target);
        target.SetOwner(ownedBy);
        StartCoroutine(target.AddNeutronWithDelay(neutron));
        neutrons.Remove(neutron);
    }
}