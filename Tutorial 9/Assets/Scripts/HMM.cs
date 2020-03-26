using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HMM : MonoBehaviour
{
    public List<char> Observables = new List<char>()
    {
        'H', 'G', 'H', 'G', 'H', 'G',
    };
    public Text ObservablesText;
    public Text States;

    // Start is called before the first frame update
    void Start()
    {
        ObservablesText.text = CharListToString(Observables);
        var states = Viterbi.GetPath(Observables);
        States.text = CharListToString(states);
    }

    private string CharListToString(List<char> charList)
    {
        var sb = new StringBuilder();
        foreach (var c in charList)
        {
            sb.Append(c);
        }

        return sb.ToString();
    }
}
