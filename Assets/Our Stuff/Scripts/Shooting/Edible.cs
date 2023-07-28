using UnityEngine;

public class Edible : MonoBehaviour , IpooledObject
{
    public enum Food 
    {
      Falafel,
      Eggplant,
      Fries
    }
    public Food FoodType;
    public bool Adible;
    private GameManager _gm;
    [SerializeField] private GameObject _chiliTrail;
    private TrailRenderer[] _chiliTrails = new TrailRenderer[4];

    private void OnCollisionEnter(Collision collision)
    {
        if (!Adible) 
            return;

        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null )
        {
            enemy.Eat(FoodType);
            gameObject.SetActive(false);
        }
        else if (collision.transform.tag == "Floor")
        {
            Adible = false;
        }
    }

    public void OnObjectSpawn()
    {
        Adible = true;
        if (_gm == null)
        {
            _gm = GameManager.Instance;
        }

        bool chili = _gm.UsedChili;
        _chiliTrail.SetActive(chili);
        if (chili)
        {
            if (_chiliTrails[0] == null)
            {
                for (int i = 0; i < 4; i++)
                {
                    _chiliTrails[i] = _chiliTrail.transform.GetChild(i).GetComponent<TrailRenderer>();
                }
            }
            foreach(TrailRenderer renderer in _chiliTrails)
            {
                renderer.Clear();
            }
        }
    }
}
