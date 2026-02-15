# Funcionamento do sistema de bate-papo em C#

O sistema funciona com um **servidor central** que coordena tudo e vários processos independentes (clientes) que conectam nele.

A comunicação é feita usando **socket TCP persistente**: depois que o cliente conecta, a conexão continua aberta e as mensagens passam continuamente pelo mesmo canal (não abre uma conexão nova para cada mensagem).

O **TCP (Transmission Control Protocol)** é um protocolo de comunicação confiável que garante que todas as mensagens enviadas pelo cliente cheguem ao servidor na ordem correta, sem perda ou duplicação. Ele estabelece um canal contínuo entre os programas, permitindo que o chat funcione em tempo real, com envio e recebimento de mensagens simultâneo.

O servidor não conversa nem interpreta as mensagens.

Ele funciona apenas como um **replicador/roteador**.

Ou seja, toda a lógica de conversa está nos clientes, e o servidor só distribui os dados.

![whasLucas](https://github.com/user-attachments/assets/1b88220c-e854-41d0-8d64-a6a3a0565505)

---

# Arquitetura

A arquitetura é do tipo **estrela**.

Nenhum cliente conhece outro cliente diretamente.

Todos conhecem apenas o servidor.

<img width="1024" height="720" alt="estrela" src="https://github.com/user-attachments/assets/7e5af59d-5e4e-42ec-973c-10b1259faf77" />

---

# Fluxo interno do servidor

O servidor fica constantemente aguardando alguém conectar.

O método `AcceptTcpClient()` é **bloqueante**, então o programa literalmente para e fica esperando até aparecer uma conexão.

Quando um cliente conecta, o servidor cria uma **thread exclusiva para ele**.

Sem isso aconteceria o seguinte:

- o servidor ficaria preso lendo apenas um cliente  
- nenhum outro conseguiria entrar

Com threads, cada cliente ganha seu próprio atendimento.

Cada thread fica em loop infinito esperando mensagem do cliente e envia para o servidor, e o servidor replica para todos. Na prática o servidor só recebe dados e redistribui.

---

# Funcionamento do cliente

Ele precisa **ler e escrever ao mesmo tempo**. Só que o método `Console.ReadLine()` bloqueia esperando o teclado e o método `StreamReader.ReadLine()` bloqueia esperando a rede. Ou seja, se usar uma execução única, ele vira um “rádio”: um fala e o outro espera, só depois responde.

Para resolver isso o cliente usa execução paralela.

A **thread principal** envia a mensagem e a **thread secundária** recebe a mensagem, assim o chat vira comunicação simultânea.

---

# Limitações atuais da arquitetura

O sistema usa modelo **1 thread por cliente**.

Isso funciona bem para poucos usuários, mas cria limitações de recursos, fazendo existir um limite prático de conexões simultâneas, que ainda não foi testado o limite máximo.

Outro ponto é que o servidor roda no meu próprio computador, então não é um servidor dedicado.
