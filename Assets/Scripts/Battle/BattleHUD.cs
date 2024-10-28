using System.Collections;
using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hPBar;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Name;
        levelText.text = "Lvl " + pokemon.Level.ToString();
        hPBar.SetHP((float)pokemon.HP / pokemon.MaxHP);
    }

    public IEnumerator UpdateData()
    {
        yield return StartCoroutine(hPBar.SeySmoothHP((float)_pokemon.HP / _pokemon.MaxHP));
    }
}
