using UnityEngine;

public class ScoreComponent : MonoBehaviour
{
    public int scoreAmount;
    private ParticleSystem ps;

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();
        if (e != null) e.DieEvent += OnDeath;

        if (ps != null)
        {
            ParticleSystem.Burst burst = ps.emission.GetBurst(0);
            burst.minCount = (short)(2 * scoreAmount);
            burst.maxCount = (short)(5 * scoreAmount);
        }
    }


    public void OnDeath(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("score");
        AddScore();
    }

    public void AddScore()
    {
        UI.UIController.Instance.AddScore(scoreAmount);
        if (ps != null) ps.Play();
    }
}
