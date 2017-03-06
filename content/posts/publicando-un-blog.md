---
date: 2017-03-01
draft: true
title: Publicando un Blog
description: Sobre el inicio y el proceso de preparación del blog.
thumbnail: posts/images/sunrise-165094_1280.jpg
---

Después evaluar durante varios meses la posibilidad de dedicarle tiempo a escribir un blog, finalmente me decidí, en buena medida por la recomendación de Santiago Porras ([@saintwukong](https://twitter.com/saintwukong)).

Santiago me comentaba que es una buena forma de tener siempre a la mano soluciones que has encontrado, además de que también le puedan servir a alguien más, como las innumerables veces que hemos aprovechado el conocimiento compartido por otros.

También me decía que es un medio de promoción profesional y que, en más de una ocasión, le había servido para conseguir oportunidades de trabajo o alguna consultoría, así que aquí estamos.

Este blog va a estar dedicado, fundamentalmente a temas de desarrollo o relacionados, donde habrá mucho código que estará publicado en repositorios en GitHub.

En el resto del artículo cuento los aspectos principales del proceso de preparación hasta la publicación final.

## ¿Inglés o Español?

Estuve pensando sobre esto algún tiempo y al final, considerando también una recomendación de Santiago, decidí publicar principalmente en español, porque ya hay suficiente contenido de este tipo en inglés. 

No descarto publicar artículos en inglés, pero, en todo caso, siempre estarán en español.

En cuanto al código, sin embargo, siempre estará en inglés, porque me parece más conciso y me resulta más acorde con los comandos y las librerías de los lenguajes. También supongo que la mayoría de los desarrolladores deben entender inglés al menos a ese nivel.

## ¿Qué herramienta utilizar?

El paso siguiente era decidir que herramienta utilizar para editar el blog.

La primera opción que evalué fue WordPress, porque, aunque no me gusta PHP, en realidad lo iba a utilizar como simple usuario, pero leí algunos artículos donde hablaban de muchas limitaciones, así que entre lo primero y esto lo deseché rápidamente (creo que más por lo primero).

En algún momento me encontré con HUGO (https://gohugo.io) y me llamó la atención eso de sitios estáticos generados justo antes de la publicación, que no requieren una base de datos y se pueden publicar en cualquier sitio, además, está desarrollado en Go (https://golang.org) y me llamó la atención la posibilidad de ir aprendiendo algo de Go y poder hacer algún hack.

Por otro lado, la edición se realiza en [Markdown](https://en.wikipedia.org/wiki/Markdown), y aunque no es tan cómodo como un editor de texto enriquecido, tampoco me molesta, así que finalmente me decidí por [HUGO](https://gohugo.io).

[HUGO](https://gohugo.io) también tiene la posibilidad de incluir [Google Analytics](https://analytics.google.com) y comentarios en cada artículo usando [Disqus](https://disqus.com/).

Sí me costó bastante tiempo encontrar una plantilla para el sitio, ya que [HUGO](https://gohugo.io) no viene con una por default y después de probar una media docena de ellas, opté por [Robust](http://themes.gohugo.io/robust), que además utiliza [Bootstrap](http://getbootstrap.com) y sería más fácil hacer algunos ajustes.

En la primera semana de trabajar con [HUGO](https://gohugo.io) encontré una forma muy sencilla de incluir el código de los programas en los artículos, leyendo el archivo directamente del repositorio y eso me confirmó que había sido una buena decisión.

Estoy claro que estaba sufriendo un caso de [sesgo de confirmación](https://es.wikipedia.org/wiki/Sesgo_de_confirmaci%C3%B3n) ("Confirmation bias"), pero de todas formas me pareció de mucho valor porque simplifica el trabajo de forma importante.

## ¿Dónde publicar?

## ¿Qué herramienta utilizar?