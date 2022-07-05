using deVoid.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace deVoid.UIFramework.Examples
{
    public class PopupExampleWindowController : AWindowController
    {
        [SerializeField] 
        private Image exampleImage = null;
        
        private int currentPopupExample;
        private Color originalColor;

        /// <summary>
        /// You can use all of Unity's regular functions, as Screens
        /// are all MonoBehaviours, but don't forget that many of them
        /// have important operations being called in their base methods
        /// </summary>
        protected override void Awake() {
            base.Awake();
            originalColor = exampleImage.color;
        }

        public void UI_ShowPopup() {
            Signals.Get<ShowConfirmationPopupSignal>().Dispatch(GetPopupData());
        }

        private ConfirmationPopupProperties GetPopupData() {
            ConfirmationPopupProperties testProps = null;
            
            switch (currentPopupExample) {
                case 0:
                    testProps = new ConfirmationPopupProperties("Uh-oh!", 
                        "You were curious and clicked the button! Try a few more times.",
                        "Got it!");
                    break;
                case 1:
                    testProps = new ConfirmationPopupProperties("Question:", 
                        "What is your favourite color?",
                        "Blue", OnBlueSelected, 
                        "Red", OnRedSelected);
                    break;
                case 2:
                    testProps = new ConfirmationPopupProperties("Pretty cool huh?",
                        "Let's return our buddy to its original color.",
                        "Fine.", OnRevertColors);
                    break;
                case 3:
                    testProps = new ConfirmationPopupProperties("YOU DIED", 
                        "The Dark Souls of Pop-Ups", "Respawn");
                    break;
            }

            currentPopupExample++;
            if (currentPopupExample > 3) {
                currentPopupExample = 0;
            }

            return testProps;
        }

        private void OnRevertColors() {
            exampleImage.color = originalColor;
        }

        private void OnRedSelected() {
            exampleImage.color = Color.red;
        }

        private void OnBlueSelected() {
            exampleImage.color = Color.blue;
        }
    }
}