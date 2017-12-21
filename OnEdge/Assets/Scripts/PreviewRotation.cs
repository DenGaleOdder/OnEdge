using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRotation : Photon.PunBehaviour
{

    public float rotationSpeed = 2f;
    Quaternion target;
    private float xDeg;
    private float yDeg;
    private Quaternion fromRotation;
    private Quaternion toRotation;
    private float speed = 6;
    private float lerpSpeed = 3;

    // Use this for initialization
    void Start () {
        target = Quaternion.Euler(0, 270, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
            if (Input.GetMouseButton(0))
            {
                yDeg -= Input.GetAxis("Mouse X") * speed;
                toRotation = Quaternion.Euler(transform.rotation.x, yDeg, transform.rotation.z);
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, toRotation, 1);
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, target, 0.01f);
                yDeg = transform.eulerAngles.y;
            }
        }
    }

    void Rotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
            // Determine the target rotation.  This is the rotation if the transform looks at the target point.

        //(Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            Quaternion targetRotation = Quaternion.Euler(0, hitdist * 10, 0);
        //targetRotation *= Quaternion.Euler(0, 90, 0);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
