using Code.Objects.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Objects
{
    public class ProfileComponent : MonoBehaviour
    {
        public User User;
        public Text Text;
        
        public void OnMouseDown()
        {

        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                var lobby = FindObjectOfType<Lobby>();

                foreach (var dict in lobby._profiles)
                {
                    dict.Item2.color = Color.magenta;
                }

                Text.color = Color.green;
                
                lobby.SelectedProfile = User;
            }
        }
    }
}