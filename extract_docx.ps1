\ = [System.IO.Compression.ZipFile]::OpenRead('C:\Users\loren\OneDrive\Documentos\GitHub\Grupo-N-5-SistemaHelpDesk\Documentos\Tp-entrega\Casos de uso. Practica Profecionalizante.docx')
\ = \.GetEntry('word/document.xml')
\ = \.Open()
\ = New-Object System.IO.StreamReader(\)
\ = \.ReadToEnd()
\.Close()
\.Close()
\.Dispose()
\ = \ -replace '<w:p[^>]*>', " 
\
\ = \ -replace '<w:r[^>]*>', ''
\ = \ -replace '</w:r>', ''
\ = \ -replace '<w:t[^>]*>', ''
\ = \ -replace '</w:t>', ''
\ = \ -replace '<[^>]+>', ''
Write-Output \
