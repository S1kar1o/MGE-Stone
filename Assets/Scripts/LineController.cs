using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static TurnManager;

public class LineController : MonoBehaviour
{
    void Start()
    {
        TurnManager.Instance.TurnChanged += OnTurnChanged;
    }

    private async void OnTurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if (e.state != TurnState.Fighting) return;

        foreach (Transform child in transform)
        {
            LineScript lineScript = child.GetComponent<LineScript>();
            if (lineScript != null)
            {
                await lineScript.StartAttack();
            }
        }

        // ��������� ��������� ��� ���� ���������� ����
        // ��� ������ �� ����� �������� ��������, ��� �� ��������� TurnChanged ����������
        await Task.Yield(); // ���� ���������� �����
        TurnManager.Instance.StartNextTurn();
    }
}
