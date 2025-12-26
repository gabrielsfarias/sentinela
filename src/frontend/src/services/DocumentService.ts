// Se você não criar um dtoTypes.ts, importe diretamente dos arquivos DTO do backend (menos ideal)
// ou defina interfaces equivalentes aqui no frontend.
// Por agora, vamos definir interfaces equivalentes aqui para simplicidade.

import axios from "axios";

export interface FeDocumentDto { // Frontend-specific DTO
    id: string;
    originalFileName: string;
    originalFileSize?: number;
    originalFileLastModified?: string; // ISO Date string
    displayName: string;
    expiryDate?: string; // ISO Date string
    notes?: string;
    createdAt: string; // ISO Date string
    updatedAt: string; // ISO Date string
}

export interface FeCreateDocumentDto {
    originalFileName: string;
    originalFileSize?: number;
    originalFileLastModified?: string; // Date.toISOString()
    displayName?: string;
    expiryDate?: string; // Date.toISOString() ou null
    notes?: string;
}

export interface FeUpdateDocumentDto {
    displayName?: string;
    expiryDate?: string | null; // Permitir null para limpar a data
    notes?: string | null; // Permitir null para limpar
}

const API_DOCUMENTS_URL = '/documentos'; // O proxy do Vite cuidará disso

export const getDocuments = async (): Promise<FeDocumentDto[]> => {
    const response = await axios.get<FeDocumentDto[]>(API_DOCUMENTS_URL);
    return response.data;
};

export const createDocumentsMetadata = async (metadataList: FeCreateDocumentDto[]): Promise<FeDocumentDto[]> => {
    const response = await axios.post<FeDocumentDto[]>(API_DOCUMENTS_URL, metadataList);
    return response.data;
};

// Funções PATCH e DELETE virão depois
// export const updateDocumentMetadata = async (id: string, data: FeUpdateDocumentDto): Promise<FeDocumentDto> => {
//     const response = await axios.patch<FeDocumentDto>(`${API_DOCUMENTS_URL}/${id}`, data);
//     return response.data;
// };

// export const deleteDocumentMetadata = async (id: string): Promise<void> => {
//     await axios.delete(`${API_DOCUMENTS_URL}/${id}`);
// };
