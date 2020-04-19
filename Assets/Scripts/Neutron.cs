using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutron : MonoBehaviour
{
    private Transform target;
    public float closeDistance = 0.3f;

    private Rigidbody _rigidbody;

    private MeshRenderer _meshRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        _rigidbody.AddForce(target.position-transform.position);
        if (Vector3.Distance(target.position, transform.position) < closeDistance)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            _rigidbody.AddForce(Vector3.Cross(_rigidbody.velocity, Vector3.up)*.3f);
            var position = transform.position;
            var ycorrection = position;
            ycorrection.y = Mathf.Lerp(position.y, target.position.y, 0.1f);
            position = ycorrection;
            transform.position = position;
        }
        else
        {
            GameManager.Instance.ResetTimer();
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        GameManager.Instance.ResetTimer();
    }
    
    public void SetOwner(Player player)
    {
        _meshRenderer.material = player.playerMat;
    }
}
