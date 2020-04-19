using UnityEngine;

public class Player : MonoBehaviour
{
    public string name;

    public Material playerMat;

    public bool isActive;

    public Neutron newNeutron;

    private Neutron _newNeutron;
    public Transform NeutronGroupTransform;
    // Start is called before the first frame update
    void Start()
    {
        //SpawnNewNeutron();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpawnNewNeutron()
    {
        GameObject go = Instantiate(GameManager.Instance.NeutronPrefab, new Vector3(4, 4, 2), Quaternion.identity, NeutronGroupTransform); 
        newNeutron = go.GetComponent<Neutron>();
        newNeutron.SetOwner(this);
    }

    public void UseNeutron()
    {
        newNeutron = null;
    }

    public void Activate()
    {
        isActive = true;
        SpawnNewNeutron();
    }
}
