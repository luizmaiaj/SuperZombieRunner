using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{
    public float offset = 16f;
    public delegate void OnDestroy();
    public event OnDestroy DescroyCallback;

    private float offscreenX = 0;
    private Rigidbody2D body2d;

    private void Awake()
    {
        body2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        offscreenX = (Screen.width / PixelPerfectCamera.pixelsToUnits) / 2 + offset;
    }

    // Update is called once per frame
    void Update()
    {
        var posX = transform.position.x;
        var dirX = body2d.velocity.x;

        // if to the left or to the right of the screen + offset then destroy object
        if((dirX < 0 && posX < -offscreenX) || (dirX > 0 && posX > offscreenX)) OnOutOfBounds();
    }

    public void OnOutOfBounds() 
    {
        GameObjectUtil.Destroy(gameObject);

        if(DescroyCallback != null)
        {
            DescroyCallback();
        }
    }
}
