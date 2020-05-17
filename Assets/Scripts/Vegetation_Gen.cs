
using UnityEngine;

public class Vegetation_Gen : MonoBehaviour
{
    public GameObject tree;
    public float minTreeSize;
    public float maxTreeSize;
    public Texture2D noiseImage;
    public float forestSize;
    public float treeDensity;

    private float baseDensity = 5.0f;

    public int pos_x;
    public int pos_z;

    // Use this for initialization
    void Start()
    {
        Generate();
    }

    public void Generate()
    {

        for (int z = pos_z; z < 300 + pos_z; z+=5)
        {
            for (int x = pos_x; x < 70; x+=5)
            {
                float chance = noiseImage.GetPixel(x, z).r / (baseDensity / treeDensity);
                if (ShouldPlaceTree(chance))
                {
                    float size = Random.Range(minTreeSize, maxTreeSize);

                    GameObject newTree = Instantiate(tree);
                    newTree.transform.localScale = Vector3.one * size;
                    newTree.transform.position = new Vector3(x, 0, z);
                    newTree.transform.parent = transform;
                }
            }
        }
    }

    //Returns true or false given some chance from 0 to 1
    public bool ShouldPlaceTree(float chance)
    {
        if (Random.Range(0.0f, 1.0f) <= chance)
        {
            return true;
        }
        return false;
    }
}
