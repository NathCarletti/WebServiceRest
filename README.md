# WebServiceRest

EndPoints:

Produtos:

GET/POST: /api/Products

PUT/DELETE/GET(id): /api/Products/:id


Pedidos:

GET/POST: /api/Orders

DELETE/GET(id): /api/Orders/:id

GET(email): /api/Orders/byEmail?email={email}

PUT (Calcular frete): /api/Orders/frete?id={id}

PUT (Fechar pedido): /api/Orders/closed?id={id}

Token: /token
Register: /api/Account/Register