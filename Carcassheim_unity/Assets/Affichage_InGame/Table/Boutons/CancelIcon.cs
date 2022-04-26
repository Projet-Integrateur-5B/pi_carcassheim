using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CancelIcon : MonoBehaviour
{

        public Button btn;
        // Start is called before the first frame update
        void Start()
        {
                Transform t = GameObject.Find("Boutons").transform;
                // btn = t.gameObject.Find("Cancel").GetComponent<Button>();

                Debug.Log("coucou " + t);

        }

        // Update is called once per frame
        void Update()
        {

        }

        /*         public void display()
                {
                        model_renderer.enabled = true;
                        if (state == TileIndicatorState.TilePossibilitie)
                                tile_collider.enabled = true;
                }


                public void hide()
                {
                        model_renderer.enabled = false;
                        if (state == TileIndicatorState.TilePossibilitie)
                                tile_collider.enabled = false;
                } */
}
