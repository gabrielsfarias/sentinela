import React, { useEffect, useState, useCallback } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import { getDocuments, createDocumentsMetadata, type FeCreateDocumentDto, type FeDocumentDto } from '../services/DocumentService';
import { useDropzone } from 'react-dropzone';


const DashboardPage: React.FC = () => {
  const { userEmail, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [documents, setDocuments] = useState<FeDocumentDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchDocuments = useCallback(async () => {
    if (!isAuthenticated) return; // Não buscar se não estiver autenticado
    setIsLoading(true);
    setError(null);
    try {
      const docs = await getDocuments();
      setDocuments(docs);
    } catch (err) {
      console.error("Erro ao buscar documentos:", err);
      setError("Não foi possível carregar os documentos.");
      // O interceptor 401 no AuthContext deve cuidar do deslogue se for o caso
    } finally {
      setIsLoading(false);
    }
  }, [isAuthenticated]); // Dependência para refazer a busca se o estado de auth mudar

  useEffect(() => {
    fetchDocuments();
  }, [fetchDocuments]); // Chama ao montar e se fetchDocuments mudar

  const onDrop = useCallback(async (acceptedFiles: File[]) => {
    if (!acceptedFiles.length) return;
    setIsLoading(true); // Pode ter um loading específico para upload
    setError(null);

    const metadataList: FeCreateDocumentDto[] = acceptedFiles.map(file => ({
      originalFileName: file.name,
      originalFileSize: file.size,
      // Converta a data para string ISO ou timestamp.
      // file.lastModified é um timestamp (milisegundos desde a época).
      // O backend espera DateTime, então uma string ISO é segura.
      originalFileLastModified: new Date(file.lastModified).toISOString(),
      displayName: file.name, // Inicializa displayName com o nome do arquivo
      // expiryDate e notes podem ser preenchidos depois ou se extraídos
    }));

    try {
      const newDocs = await createDocumentsMetadata(metadataList);
      // Adiciona os novos documentos à lista existente ou recarrega tudo
      // Para simplicidade, vamos recarregar tudo. Para UX melhor, adicione à lista.
      // setDocuments(prevDocs => [...prevDocs, ...newDocs].sort((a,b) => Date.parse(b.updatedAt) - Date.parse(a.updatedAt)));
      fetchDocuments(); // Recarrega a lista para incluir os novos
      console.log("Documentos criados:", newDocs);
    } catch (err) {
      console.error("Erro ao criar metadados de documentos:", err);
      setError("Falha ao registrar os novos documentos.");
    } finally {
      setIsLoading(false);
    }
  }, [fetchDocuments]); // Adicionar fetchDocuments como dependência

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    // accept: { 'application/pdf': ['.pdf'], 'image/*': ['.jpeg', '.jpg', '.png'] } // Exemplo de filtro de tipo
  });

  const handleLogout = () => {
    logout({ navigate });
  };

  // --- Lógica de Ordenação e Filtragem (Simples Exemplo) ---
  const getUpcomingExpiryDocuments = (days: number = 30): FeDocumentDto[] => {
    const futuro = new Date();
    futuro.setDate(futuro.getDate() + days);
    return documents
      .filter(doc => doc.expiryDate && new Date(doc.expiryDate) <= futuro && new Date(doc.expiryDate) >= new Date())
      .sort((a, b) => new Date(a.expiryDate!).getTime() - new Date(b.expiryDate!).getTime());
  };

  const upcomingDocuments = getUpcomingExpiryDocuments(30); // Documentos vencendo nos próximos 30 dias

  if (isLoading && !documents.length) { // Mostra loading apenas se não houver documentos ainda
    return <div>Carregando documentos...</div>;
  }

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h1>Dashboard de Documentos</h1>
        <div>
          {userEmail && <span style={{ marginRight: '15px' }}>Bem-vindo, {userEmail}!</span>}
          <Link to="/account/change-password" style={{ marginRight: '10px' }}>Alterar Senha</Link>
          <button onClick={handleLogout}>Sair</button>
        </div>
      </div>

      {/* --- ÁREA DE DROPZONE E TABELA --- */}
      <div
        {...getRootProps()}
        style={{
          border: `2px dashed ${isDragActive ? 'green' : '#ccc'}`,
          padding: '20px',
          textAlign: 'center',
          marginBottom: '20px',
          backgroundColor: isDragActive ? '#f0fff0' : '#f9f9f9'
        }}
      >
        <input {...getInputProps()} />
        {isDragActive ? (
          <p>Solte os arquivos aqui...</p>
        ) : (
          <p>Arraste e solte alguns arquivos aqui, ou clique para selecionar arquivos</p>
        )}
      </div>

      {error && <p style={{ color: 'red' }}>Erro: {error}</p>}

      {/* Seção de Documentos Próximos do Vencimento */}
      {upcomingDocuments.length > 0 && (
        <div style={{ marginBottom: '20px' }}>
          <h3>⚠️ Próximos do Vencimento (30 dias)</h3>
          <ul style={{ listStyleType: 'none', paddingLeft: 0 }}>
            {upcomingDocuments.map(doc => (
              <li key={doc.id} style={{ padding: '5px 0', borderBottom: '1px solid #eee' }}>
                <strong>{doc.displayName}</strong> - Vence em: {new Date(doc.expiryDate!).toLocaleDateString()}
              </li>
            ))}
          </ul>
        </div>
      )}


      <h3>Todos os Documentos</h3>
      {documents.length === 0 && !isLoading ? (
        <p>Nenhum documento registrado ainda. Arraste arquivos para a área acima para começar.</p>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ borderBottom: '2px solid #ddd', textAlign: 'left' }}>
              <th style={{ padding: '8px' }}>Nome Exibição</th>
              <th style={{ padding: '8px' }}>Data Validade</th>
              <th style={{ padding: '8px' }}>Nome Original</th>
              <th style={{ padding: '8px' }}>Notas</th>
              <th style={{ padding: '8px' }}>Ações</th>
            </tr>
          </thead>
          <tbody>
            {documents.map((doc) => (
              <tr key={doc.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: '8px' }}>{doc.displayName}</td>
                <td style={{ padding: '8px' }}>
                  {doc.expiryDate ? new Date(doc.expiryDate).toLocaleDateString() : 'N/A'}
                </td>
                <td style={{ padding: '8px' }}>{doc.originalFileName}</td>
                <td style={{ padding: '8px' }}>{doc.notes || '---'}</td>
                <td style={{ padding: '8px' }}>
                  {/* Botões/Links de Ação (Editar/Excluir) virão aqui */}
                  <button onClick={() => alert(`Editar ${doc.id}`)} style={{ marginRight: '5px' }}>Editar</button>
                  <button onClick={() => alert(`Excluir ${doc.id}`)}>Excluir</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
      {isLoading && documents.length > 0 && <p>Atualizando lista...</p>}
    </div>
  );
};

export default DashboardPage;