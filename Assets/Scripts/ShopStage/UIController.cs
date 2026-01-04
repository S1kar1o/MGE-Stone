using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ShopStage
{
    internal class UIController: MonoBehaviour
    {
        [SerializeField]private Button returnButton;

        [SerializeField] private RequestForBuyCard buyCard;
            [SerializeField]private Button rollOneButton;
            [SerializeField]private Button rollTenButton;

        [SerializeField]private Button claimRewardsButton;
        public EventHandler rollAgainButtonPressed;

        [SerializeField] private string nameOfSceneToReturn;

        [SerializeField] private Transform containerForx10Summon;
        [SerializeField] private Transform containerOneSummon;
        [SerializeField] private Transform rewardGroupTransform;

        [SerializeField] private RewardGroupUIController rewardGroupController;
        private void Start()
        {
            buyCard.SetUpButtons(rollOneButton,rollTenButton);
            
            buyCard.SetUpButtons(rewardGroupController.rollAgainOne, rewardGroupController.rollAgainTen);

            returnButton.onClick.AddListener(() => {
                SceneLoader.Instance.LoadScene(nameOfSceneToReturn);
            });

            rollOneButton.onClick.AddListener(() => {
                rewardGroupTransform.gameObject.SetActive(true);
            });


            rollTenButton.onClick.AddListener(() => {
                rewardGroupTransform.gameObject.SetActive(true);
            });

        }

    }
}
