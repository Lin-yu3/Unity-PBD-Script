using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public GameObject NP;
    //public GameObject SP;
    private static float mu0 = 10.0f;
    public float strength;
    private Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 磁場
    void OnTriggerStay(Collider other)
    {

        Rigidbody other_rb = other.gameObject.GetComponent<Rigidbody>();

        // 判斷標籤 "沒磁性"
        if (other.gameObject.tag == "Paramagnetic")
        {
            // r_n : 從N磁极到目標物的位移向量
            // s_n : 從S磁极到目標物的位移向量
            Vector3 r_n = other.gameObject.transform.position - NP.transform.position; // displacement vector
            //Vector3 r_s = other.gameObject.transform.position - SP.transform.position; // displacement vector
            // 磁鐵的磁場強度
            float mu = Time.fixedDeltaTime * strength * mu0 * other_rb.mass;
            // 計算磁力大小
            // f_n : N極對物體施的力
            // f_s : S極對物體施的力
            Vector3 f_n = (r_n.normalized) * mu / (Mathf.Pow(Mathf.Max(r_n.magnitude, 0.2f), 3));
            //Vector3 f_s = (r_s.normalized) * mu / (Mathf.Pow(Mathf.Max(r_s.magnitude, 0.2f), 3));

            //other_rb : 同磁鐵相互作用的目標物体的rigidbody。
            other_rb.AddForce(-1f * f_n);
            //other_rb.AddForce(-1f * f_s);
            //對磁鐵的吸引力
            rb.AddForceAtPosition(1f * f_n, NP.transform.position);
            //rb.AddForceAtPosition(1f * f_s, SP.transform.position);
        }
        // 判斷標籤 "磁性"
        if (other.gameObject.tag == "Magnetic")
        {
            MagneticField other_script = other.gameObject.GetComponent<MagneticField>();
            GameObject other_NP = other_script.NP;
            //GameObject other_SP = other_script.SP;
            Vector3 r_n_n = other_NP.transform.position - NP.transform.position;
            // from this(N) to that(N)
            //Vector3 r_s_s = other_SP.transform.position - SP.transform.position; // from this(S) to that(S)
            // Vector3 r_n_s = other_SP.transform.position - NP.transform.position; // from this(N) to that(S)
            //Vector3 r_s_n = other_NP.transform.position - SP.transform.position; // from this(S) to that(N)
            float other_strength = other_script.strength;

            // 計算兩極的力
            float mu = Time.fixedDeltaTime * strength * other_strength;
            // from this(N) to that(N)
            Vector3 f_n_n = (r_n_n.normalized) * mu / (Mathf.Pow(Mathf.Max(r_n_n.magnitude, 0.2f), 3));
            // from this(S) to that(S)
            //Vector3 f_s_s = (r_s_s.normalized) * mu / (Mathf.Pow(Mathf.Max(r_s_s.magnitude, 0.2f), 3));
            // from this(N) to that(S)
            //Vector3 f_n_s = (r_n_s.normalized) * mu / (Mathf.Pow(Mathf.Max(r_n_s.magnitude, 0.2f), 3));
            // from this(S) to that(N)
            //Vector3 f_s_n = (r_s_n.normalized) * mu / (Mathf.Pow(Mathf.Max(r_s_n.magnitude, 0.2f), 3));
        }
    }
}


