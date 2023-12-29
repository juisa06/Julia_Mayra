using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float velocidadeMovimento = 3f;
    public float distanciaPerseguicao = 8f;
    public float intervaloTiros = 2f;
    public float distanciaOuvirTiros = 10f;
    public float intervaloTrocaMovimento = 3f;
    public GameObject projetilPrefab;
    public Transform pontoDeTiro;
    public float velocidadeRotacao = 5f;
    private Transform jogador;
    private Rigidbody2D rb;
    private float tempoDesdeUltimoTiro;
    private bool emMovimentoAleatorio = false;
    private Vector2 direcaoAleatoria;
    private float tempoDesdeUltimaTrocaMovimento;
    private bool emAtaque = false;

    private Transform spriteTransform; // Objeto filho usado para controlar a rotação do sprite

    void Start()
    {
        jogador = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        tempoDesdeUltimaTrocaMovimento = Time.time;

        // Encontrar o objeto filho para controlar a rotação do sprite
        spriteTransform = transform.Find("SpritePivot");
        if (spriteTransform == null)
        {
            Debug.LogError("Objeto filho 'SpritePivot' não encontrado. Crie um objeto filho chamado 'SpritePivot' para controlar a rotação do sprite.");
        }
    }

    void Update()
    {
        if (!emMovimentoAleatorio)
        {
            if (!emAtaque)
            {
                VerificarVisaoJogador();
            }
            else
            {
                Atacar();
            }
        }
        else
        {
            MoverAleatoriamente();
        }

        if (Time.time - tempoDesdeUltimaTrocaMovimento > intervaloTrocaMovimento)
        {
            emMovimentoAleatorio = !emMovimentoAleatorio;
            tempoDesdeUltimaTrocaMovimento = Time.time;

            if (emAtaque && Vector3.Distance(transform.position, jogador.position) > distanciaPerseguicao)
            {
                emAtaque = false;
            }
        }
    }

    void Mover()
    {
        Vector2 direcao = jogador.position - transform.position;
        direcao.Normalize();

        rb.velocity = direcao * velocidadeMovimento;

        // Rotacionar o sprite na direção do jogador
        spriteTransform.up = direcao;
    }

    void MoverAleatoriamente()
    {
        rb.velocity = direcaoAleatoria * velocidadeMovimento;
    }

    void VerificarVisaoJogador()
    {
        if (Vector3.Distance(transform.position, jogador.position) < distanciaPerseguicao)
        {
            rb.velocity = Vector2.zero;
            emAtaque = true;
        }
        else if (Vector3.Distance(transform.position, jogador.position) < distanciaOuvirTiros)
        {
            emMovimentoAleatorio = false;
        }

        if (emAtaque == false)
        {
            emMovimentoAleatorio = true;
            direcaoAleatoria = Random.insideUnitCircle.normalized;
        }
    }

    void Atacar()
    {
        if (Time.time - tempoDesdeUltimoTiro > intervaloTiros)
        {
            StartCoroutine("Atirar");
            tempoDesdeUltimoTiro = Time.time;
        }

        Vector2 direcaoAtaque = jogador.position - transform.position;
        direcaoAtaque.Normalize();

        // Calcular o ângulo em radianos
        float angle = Mathf.Atan2(direcaoAtaque.y, direcaoAtaque.x) * Mathf.Rad2Deg;

        // Atualizar a rotação do sprite no eixo Z
        spriteTransform.localEulerAngles = new Vector3(0, 0, angle);

        if (Vector3.Distance(transform.position, jogador.position) > distanciaPerseguicao)
        {
            StopCoroutine("Atacar");
            emAtaque = false;
        }
    }
    IEnumerator Atirar()
    {
        if (Vector3.Distance(transform.position, jogador.position) < distanciaPerseguicao)
        {
            emAtaque = true;
            Instantiate(projetilPrefab, pontoDeTiro.position, pontoDeTiro.rotation);
        }
        else
        {
            emAtaque = false;
        }
        yield return new WaitForSeconds(1f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            emMovimentoAleatorio = true;
            direcaoAleatoria = Random.insideUnitCircle.normalized;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaPerseguicao);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaOuvirTiros);
    }
}
