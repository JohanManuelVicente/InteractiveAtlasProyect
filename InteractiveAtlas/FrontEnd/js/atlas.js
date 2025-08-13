// Configuración de la API
const API_BASE_URL = 'https://localhost:7193/api';

// Variables globales
let allProvinces = [];
let selectedProvinceId = null;
let currentStats = {
    totalProvinces: 0,
    totalPopulation: 0
};

// ================================
// FUNCIONES DE UTILIDAD
// ================================

function showLoading() {
    console.log('Cargando datos...');
}

function hideLoading() {
    console.log('Datos cargados');
}

function showError(message) {
    console.error('Error:', message);
    
    // Mostrar toast de error
    showToast(message, 'error');
    
    // También mostrar SweetAlert para errores críticos
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: message,
        confirmButtonColor: '#3498db'
    });
}

function showSuccess(message) {
    showToast(message, 'success');
}

function showToast(message, type = 'info') {
    const toastElement = document.getElementById('notificationToast');
    const toastMessage = document.getElementById('toastMessage');
    
    // Cambiar icono según el tipo
    const toastHeader = toastElement.querySelector('.toast-header i');
    toastHeader.className = `fas me-2 ${
        type === 'error' ? 'fa-exclamation-triangle text-danger' :
        type === 'success' ? 'fa-check-circle text-success' :
        'fa-info-circle text-primary'
    }`;
    
    toastMessage.textContent = message;
    
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}

function formatNumber(number) {
    if (!number) return '-';
    return new Intl.NumberFormat('es-DO').format(number);
}

// ================================
// FUNCIONES DE API
// ================================

async function fetchProvinces() {
    try {
        showLoading();
        
        const response = await fetch(`${API_BASE_URL}/provinces`);
        
        if (!response.ok) {
            throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        const data = await response.json();
        
        hideLoading();
        return data;
    } catch (error) {
        hideLoading();
        showError('No se pudieron cargar las provincias: ' + error.message);
        return [];
    }
}

async function fetchProvinceById(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/provinces/${id}`);
        
        if (!response.ok) {
            throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        return await response.json();
    } catch (error) {
        showError('Error al cargar la provincia: ' + error.message);
        return null;
    }
}

async function fetchTouristAttractionsByProvince(provinceId) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/by-province?provinceId=${provinceId}`);
        
        if (!response.ok) {
            throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        return await response.json();
    } catch (error) {
        console.error('Error al cargar atracciones:', error);
        return [];
    }
}

async function fetchTypicalProductsByProvince(provinceId) {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/by-province?provinceId=${provinceId}`);
        
        if (!response.ok) {
            throw new Error(`Error ${response.status}: ${response.statusText}`);
        }
        
        return await response.json();
    } catch (error) {
        console.error('Error al cargar productos típicos:', error);
        return [];
    }
}

// ================================
// FUNCIONES DE CARGA DE DATOS
// ================================

async function loadProvinces() {
    try {
        const data = await fetchProvinces();
        allProvinces = data;
        
        updateGeneralStats();
        populateRegionFilter();
        
        // Mostrar mensaje de éxito
        if (data.length > 0) {
            showSuccess(`Se cargaron ${data.length} provincias correctamente`);
            
            // Ocultar loading del mapa y mostrar contenido
            document.getElementById('loadingMessage').style.display = 'none';
            document.getElementById('mapContent').style.display = 'block';
        }
        
    } catch (error) {
        console.error('Error loading provinces:', error);
    }
}

function updateGeneralStats() {
    if (allProvinces.length > 0) {
        currentStats.totalProvinces = allProvinces.length;
        currentStats.totalPopulation = allProvinces.reduce((sum, province) => sum + (province.population || 0), 0);
        
        // Actualizar la UI
        document.getElementById('totalProvinces').textContent = currentStats.totalProvinces;
        document.getElementById('totalPopulation').textContent = formatNumber(currentStats.totalPopulation);
        
        // Actualizar datos curiosos
        updateFunFacts();
    }
}

function updateFunFacts() {
    if (allProvinces.length === 0) return;
    
    // Calcular algunos datos curiosos
    const largestProvince = allProvinces.reduce((prev, current) => 
        (prev.areaKm2 > current.areaKm2) ? prev : current
    );
    
    const mostPopulated = allProvinces.reduce((prev, current) => 
        (prev.population > current.population) ? prev : current
    );
    
    const funFactsContainer = document.getElementById('funFactsContainer');
    funFactsContainer.innerHTML = `
        <div class="mb-2">
            <small class="text-muted">Provincia más grande:</small>
            <div><strong>${largestProvince.name}</strong> (${largestProvince.areaKm2.toFixed(1)} km²)</div>
        </div>
        <div>
            <small class="text-muted">Más poblada:</small>
            <div><strong>${mostPopulated.name}</strong> (${formatNumber(mostPopulated.population)} hab.)</div>
        </div>
    `;
}

// ================================
// FUNCIONES DE SELECCIÓN DE PROVINCIA
// ================================

async function selectProvince(provinceId) {
    if (!provinceId) {
        resetProvinceSelection();
        return;
    }
    
    try {
        selectedProvinceId = provinceId;
        
        // Buscar provincia en el array local primero
        let province = allProvinces.find(p => p.id === provinceId);
        
        if (!province) {
            // Si no está en local, hacer request específico
            province = await fetchProvinceById(provinceId);
        }
        
        if (province) {
            await displayProvinceDetails(province);
        }
        
    } catch (error) {
        console.error('Error selecting province:', error);
        showError('Error al cargar información de la provincia');
    }
}

async function displayProvinceDetails(province) {
    // Mostrar el contenedor de detalles
    document.getElementById('noProvinceSelected').style.display = 'none';
    document.getElementById('provinceDetailsContainer').style.display = 'block';
    
    // Actualizar información básica
    document.getElementById('provinceName').textContent = province.name;
    document.getElementById('provinceRegion').textContent = province.region;
    document.getElementById('provinceCapital').textContent = province.capital;
    document.getElementById('provincePopulation').textContent = formatNumber(province.population);
    document.getElementById('provinceArea').textContent = `${province.areaKm2.toFixed(1)} km²`;
    document.getElementById('provinceDensity').textContent = `${province.density.toFixed(1)} hab/km²`;
    
    // Cargar atracciones turísticas
    await loadProvinceAttractions(province.id);
    
    // Cargar productos típicos
    await loadProvinceProducts(province.id);
}

async function loadProvinceAttractions(provinceId) {
    const container = document.getElementById('attractionsContainer');
    
    // Mostrar loading
    container.innerHTML = `
        <div class="text-center">
            <div class="spinner-border spinner-border-sm" role="status"></div>
            <small class="d-block">Cargando atracciones...</small>
        </div>
    `;
    
    try {
        const attractions = await fetchTouristAttractionsByProvince(provinceId);
        
        if (attractions.length > 0) {
            container.innerHTML = attractions.map(attraction => `
                <div class="attraction-item p-2 mb-2 bg-light rounded">
                    <h6 class="mb-1">${attraction.name}</h6>
                    ${attraction.description ? `<small class="text-muted">${attraction.description}</small>` : ''}
                    ${attraction.location ? `<div><small><i class="fas fa-map-marker-alt"></i> ${attraction.location}</small></div>` : ''}
                </div>
            `).join('');
        } else {
            container.innerHTML = '<small class="text-muted">No hay atracciones registradas</small>';
        }
    } catch (error) {
        container.innerHTML = '<small class="text-danger">Error al cargar atracciones</small>';
    }
}

async function loadProvinceProducts(provinceId) {
    const container = document.getElementById('productsContainer');
    
    // Mostrar loading
    container.innerHTML = `
        <div class="text-center">
            <div class="spinner-border spinner-border-sm" role="status"></div>
            <small class="d-block">Cargando productos...</small>
        </div>
    `;
    
    try {
        const products = await fetchTypicalProductsByProvince(provinceId);
        
        if (products.length > 0) {
            container.innerHTML = products.map(product => `
                <div class="product-item p-2 mb-2 bg-light rounded">
                    <h6 class="mb-1">${product.name}</h6>
                    ${product.description ? `<small class="text-muted">${product.description}</small>` : ''}
                </div>
            `).join('');
        } else {
            container.innerHTML = '<small class="text-muted">No hay productos típicos registrados</small>';
        }
    } catch (error) {
        container.innerHTML = '<small class="text-danger">Error al cargar productos</small>';
    }
}

function resetProvinceSelection() {
    selectedProvinceId = null;
    document.getElementById('noProvinceSelected').style.display = 'block';
    document.getElementById('provinceDetailsContainer').style.display = 'none';
}

// ================================
// FUNCIONES DE BÚSQUEDA Y FILTROS
// ================================

function setupSearchFunctionality() {
    const searchInput = document.getElementById('searchInput');
    const modalSearchInput = document.getElementById('modalSearchInput');
    const regionFilter = document.getElementById('regionFilter');
    
    // Búsqueda en tiempo real
    searchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();
        filterProvinces(searchTerm);
    });
    
    // Búsqueda en modal
    modalSearchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();
        const regionFilter = document.getElementById('regionFilter').value;
        updateProvincesModal(searchTerm, regionFilter);
    });
    
    // Filtro por región en modal
    regionFilter.addEventListener('change', function() {
        const searchTerm = modalSearchInput.value.toLowerCase();
        const regionFilter = this.value;
        updateProvincesModal(searchTerm, regionFilter);
    });
}

function filterProvinces(searchTerm) {
    const filteredProvinces = allProvinces.filter(province =>
        province.name.toLowerCase().includes(searchTerm) ||
        province.capital.toLowerCase().includes(searchTerm) ||
        province.region.toLowerCase().includes(searchTerm)
    );
    
    console.log(`Encontradas ${filteredProvinces.length} provincias que coinciden con "${searchTerm}"`);
    
    // Aquí podrías actualizar el mapa visual si tuvieras uno
    // Por ahora solo mostramos en consola
}

function populateRegionFilter() {
    const regionFilter = document.getElementById('regionFilter');
    const regions = [...new Set(allProvinces.map(p => p.region))].sort();
    
    regionFilter.innerHTML = '<option value="">Todas las regiones</option>';
    regions.forEach(region => {
        const option = document.createElement('option');
        option.value = region;
        option.textContent = region;
        regionFilter.appendChild(option);
    });
}

// ================================
// MODAL DE PROVINCIAS
// ================================

function showProvincesModal() {
    updateProvincesModal();
    const modal = new bootstrap.Modal(document.getElementById('provincesModal'));
    modal.show();
}

function updateProvincesModal(searchTerm = '', regionFilter = '') {
    const tableBody = document.getElementById('provincesTableBody');
    
    let filteredProvinces = allProvinces;
    
    // Aplicar filtros
    if (searchTerm) {
        filteredProvinces = filteredProvinces.filter(province =>
            province.name.toLowerCase().includes(searchTerm) ||
            province.capital.toLowerCase().includes(searchTerm)
        );
    }
    
    if (regionFilter) {
        filteredProvinces = filteredProvinces.filter(province =>
            province.region === regionFilter
        );
    }
    
    // Llenar tabla
    tableBody.innerHTML = '';
    
    if (filteredProvinces.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No se encontraron provincias</td></tr>';
        return;
    }
    
    filteredProvinces.forEach(province => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td><strong>${province.name}</strong></td>
            <td>${province.capital}</td>
            <td><span class="badge bg-secondary">${province.region}</span></td>
            <td>${formatNumber(province.population)}</td>
            <td>
                <button class="btn btn-sm btn-primary" onclick="selectProvinceFromModal(${province.id})">
                    <i class="fas fa-eye"></i> Ver
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}

function selectProvinceFromModal(provinceId) {
    // Cerrar modal
    const modal = bootstrap.Modal.getInstance(document.getElementById('provincesModal'));
    modal.hide();
    
    // Seleccionar provincia
    selectProvince(provinceId);
}

// ================================
// FUNCIONES DE EVENTOS
// ================================

function setupEventListeners() {
    // Botón de actualizar datos
    document.getElementById('refreshData').addEventListener('click', async function() {
        this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Actualizando...';
        this.disabled = true;
        
        await loadProvinces();
        
        this.innerHTML = '<i class="fas fa-sync-alt"></i> Actualizar datos';
        this.disabled = false;
    });
    
    // Botón de ver todas las provincias
    document.getElementById('viewAllProvinces').addEventListener('click', showProvincesModal);
    
    // Botón del quiz
    document.getElementById('startQuizBtn').addEventListener('click', function() {
        showToast('Funcionalidad del quiz próximamente...', 'info');
    });
}

// ================================
// FUNCIONES GLOBALES (para HTML)
// ================================

// Hacer funciones disponibles globalmente para onclick en HTML
window.selectProvinceFromModal = selectProvinceFromModal;
window.selectProvince = selectProvince;

// ================================
// INICIALIZACIÓN
// ================================

async function initializeApp() {
    try {
        console.log('🚀 Inicializando Atlas Dominicano...');
        
        // Configurar event listeners
        setupEventListeners();
        setupSearchFunctionality();
        
        // Cargar datos iniciales
        await loadProvinces();
        
        console.log('✅ Atlas Dominicano inicializado correctamente');
        
    } catch (error) {
        console.error('❌ Error al inicializar la aplicación:', error);
        showError('Error al inicializar la aplicación. Por favor, recarga la página.');
    }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});