// var snippets = $('.snippet');

// for (snippet of snippets) { 
// 	snippet.firstChild.insertAdjacentHTML('beforebegin', '<button class="btn" data-clipboard-snippet><img class="clippy" width="13" src="/images/clippy.svg" alt="Copiar al clipboard"></button>');
//  }; 
 
 var clipboardSnippets = new Clipboard('[data-clipboard-snippet]', { target: function (trigger) { return trigger.nextElementSibling; } });

 clipboardSnippets.on('success', function (e) { e.clearSelection(); showTooltip(e.trigger, 'Copiado!'); }); 
 clipboardSnippets.on('error', function (e) { showTooltip(e.trigger, fallbackMessage(e.action)); });