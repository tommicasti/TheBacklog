#!/bin/bash

# Imposta l'uscita in caso di errore
set -e

# Applica le migrazioni di Entity Framework
# Il flag --no-build presuppone che il progetto sia già compilato
echo "Applicando le migrazioni del database..."
dotnet ef database update --no-build

# Esegui il comando principale del container (l'avvio dell'applicazione)