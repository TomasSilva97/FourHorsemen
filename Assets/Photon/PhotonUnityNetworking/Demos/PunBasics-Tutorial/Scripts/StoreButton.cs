
using UnityEngine;

public class StoreButton : MonoBehaviour
{
    public Transform centerStore;
    public Transform storeContainer;

    public void OnClickCharacter()
    {
        float dis = centerStore.position.x - transform.position.x;
        StoreController.newPose = new Vector3(storeContainer.position.x + dis, storeContainer.position.y, storeContainer.position.z);
        StoreController.selectMove = true;
    }

    public void SelectCharacter(int selectedCharacter)
    {
        CharacterChooser.setCharacter(selectedCharacter);
    }
}
