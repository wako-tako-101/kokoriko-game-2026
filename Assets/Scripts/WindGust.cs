using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WindGust : MonoBehaviour
{
    [Header("Gust Settings")]
    [Tooltip("Constant velocity applied to the player while the gust is active (up-and-right).")]
    public Vector2 gustVelocity = new Vector2(6f, 10f);
    [Tooltip("Seconds the gust stays inactive before turning on.")]
    public float idleDuration = 4f;
    [Tooltip("Seconds the gust stays active before turning back off.")]
    public float activeDuration = 3f;

    private bool isGusting = false;
    private Coroutine cycleRoutine;
    private SpriteRenderer spriteRenderer;

    // Ref-counts overlapping colliders per player, in case a player has more than one (e.g. body + feet).
    private readonly Dictionary<PlayerMovement, int> colliderCounts = new Dictionary<PlayerMovement, int>();
    // Players this gust has actually disabled, so we only ever re-enable control we ourselves took.
    private readonly HashSet<PlayerMovement> controlledPlayers = new HashSet<PlayerMovement>();

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    private void OnEnable()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        cycleRoutine = StartCoroutine(GustCycle());
    }

    private void OnDisable()
    {
        if (cycleRoutine != null)
        {
            StopCoroutine(cycleRoutine);
            cycleRoutine = null;
        }

        ReleaseAllPlayers();
        colliderCounts.Clear();
        isGusting = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    private IEnumerator GustCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleDuration);

            isGusting = true;
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            foreach (PlayerMovement player in colliderCounts.Keys)
                TryTakeControl(player);

            yield return new WaitForSeconds(activeDuration);

            isGusting = false;
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            ReleaseAllPlayers();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        colliderCounts.TryGetValue(player, out int count);
        colliderCounts[player] = count + 1;

        if (isGusting)
            TryTakeControl(player);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGusting) return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null || !controlledPlayers.Contains(player)) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
            rb.linearVelocity = gustVelocity;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        if (!colliderCounts.TryGetValue(player, out int count)) return;

        count--;
        if (count <= 0)
        {
            colliderCounts.Remove(player);
            ReleaseControl(player);
        }
        else
        {
            colliderCounts[player] = count;
        }
    }

    private void TryTakeControl(PlayerMovement player)
    {
        // Skip players already disabled by another system (e.g. death/respawn) -- don't stomp on unrelated state.
        if (controlledPlayers.Contains(player) || player.isDisabled) return;

        player.isDisabled = true;
        controlledPlayers.Add(player);
    }

    private void ReleaseControl(PlayerMovement player)
    {
        if (controlledPlayers.Remove(player))
            player.isDisabled = false;
    }

    private void ReleaseAllPlayers()
    {
        foreach (PlayerMovement player in controlledPlayers)
            player.isDisabled = false;
        controlledPlayers.Clear();
    }
}
