 var clipboardSnippets = new Clipboard('[data-clipboard-snippet]', { target: function (trigger) { return trigger.nextElementSibling; } });

 clipboardSnippets.on('success', function (e) { e.clearSelection(); showTooltip(e.trigger, 'Copiado!'); }); 
 clipboardSnippets.on('error', function (e) { showTooltip(e.trigger, fallbackMessage(e.action)); });