using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ShopStage
{
    internal class RewardGroupUIController : MonoBehaviour
    {
        [SerializeField] private Button claimReward;


        [SerializeField] public Button rollAgainOne;
        [SerializeField] public Button rollAgainTen; //UIController use them


        [SerializeField] private GameObject soloObjGroup;
        [SerializeField] private GameObject tenObjGroup;
        [SerializeField] private GameObject currentObjTransformActive;
        private void Start()
        {
            claimReward.onClick.AddListener(() => { gameObject.SetActive(false); });

            rollAgainOne.onClick.AddListener(() =>
            {
                gameObject.SetActive(true);
                if(currentObjTransformActive != null&& currentObjTransformActive!= soloObjGroup)
                    currentObjTransformActive.SetActive(false);
                //start anim of loading
                currentObjTransformActive = soloObjGroup;
            });

            rollAgainTen.onClick.AddListener(() =>
            {
                if (currentObjTransformActive != null && currentObjTransformActive != tenObjGroup)
                    currentObjTransformActive.SetActive(false);
                //start anim of loading
                currentObjTransformActive = tenObjGroup;
                gameObject.SetActive(true);

            });
        }

    }
}
