using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines.ExtrusionShapes;

/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    // Nombre de charge de boost
    private int nombreCharge;

    private float boost = 15;

    [SerializeField]
    private TMP_Text textBoost;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }

        if (Keyboard.current.spaceKey.isPressed)
        {
            rigidbody.useGravity = true;
            ControleurJeu.Instance.Controles.actions.FindAction("Diriger").performed += CommencerDirection;
        }

        StartCoroutine(Boost());

        MettreAJourInterface();
    }

    private void FixedUpdate()
    {
        Diriger();
    }

    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * Vector3.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void Diriger()
    {
        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boost"))
        {
            if (nombreCharge < 3)
            {
                nombreCharge++;
            }
        }
    }

    private void MettreAJourInterface()
    {
        if (textBoost != null)
        {
            textBoost.text = "Boost : " + nombreCharge;
        }
    }

    private IEnumerator Boost()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            if (nombreCharge > 0)
            {
                rigidbody.AddForce(this.transform.forward * boost, ForceMode.Acceleration);
                nombreCharge--;
                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
}
