using TDG.Entity;
using UnityEngine;


public class Shopkeep : MonoBehaviour
{
    [SerializeField] private Material highlightMat;
    private Material defaultMat;
    private Renderer renderer;
    private bool isSelected;
    [SerializeField] private Player player;

    private void Start()
    {
        defaultMat = transform.GetComponent<Renderer>().material;
        renderer = transform.GetComponent<Renderer>();

        // NOTE
        // Since shop keeps will spawn before player, this won't find the player
        // Update this later
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        isSelected = SelectionCheck();
    }

    // Checks if the player is currently looking at this shop object:
    private bool SelectionCheck()
    {
        if (player)
        {
            if (player.selectedObj.transform == transform)
            {
                renderer.material = highlightMat;
                return true;
            }

            renderer.material = defaultMat;
            return false;
        }

        Debug.Log("Cannot find object with tag 'Player'");
        return false;
    }

}
