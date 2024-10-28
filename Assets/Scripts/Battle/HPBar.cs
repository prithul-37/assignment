using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image Health;
    float _hp;
    void Start()
    {
        Health.fillAmount = 1;
    }

    public void SetHP(float hp)
    {
        Health.fillAmount = hp;
        _hp = hp; 
    }

    public IEnumerator SeySmoothHP(float newHP)
    {
        float changeAmount = _hp - newHP;
        
        while(_hp - newHP > Mathf.Epsilon)
        {
            _hp -= changeAmount * Time.deltaTime;
            Health.fillAmount = _hp;
            yield return null;       
        }

        Health.fillAmount = newHP;
        _hp = newHP; 
    }

}
