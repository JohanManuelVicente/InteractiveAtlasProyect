
const API_BASE_URL = 'https://localhost:7193/api';

// Variables globales
let allProvinces = [];
let allQuestions = [];
let allAnswers = [];
let allAttractions = [];
let allProducts = [];
let selectedProvinceId = null;
let selectedQuestionId = null;
let currentStats = {
    totalProvinces: 0,
    totalPopulation: 0
};
let isAdminMode = false;

// Variables globales del Quiz
let quizData = {
    questions: [],
    currentQuestionIndex: 0,
    userAnswers: [],
    score: 0,
    isActive: false,
    difficulty: 'Facil',
    provinceId: null
};


function showLoading() {
    console.log('Cargando datos...');
}

function hideLoading() {
    console.log('Datos cargados');
}

function showError(message) {
    console.error('Error:', message);
    showToast(message, 'error');
    
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
    
    if (!toastElement || !toastMessage) {
        console.log(message);
        return;
    }
    
    const toastHeader = toastElement.querySelector('.toast-header i');
    if (toastHeader) {
        toastHeader.className = `fas me-2 ${
            type === 'error' ? 'fa-exclamation-triangle text-danger' :
            type === 'success' ? 'fa-check-circle text-success' :
            'fa-info-circle text-primary'
        }`;
    }
    
    toastMessage.textContent = message;
    
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}

function formatNumber(number) {
    if (!number) return '-';
    return new Intl.NumberFormat('es-DO').format(number);
}


function shuffleArray(array) {
    const shuffled = [...array];
    for (let i = shuffled.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
}

function getScoreMessage(score, total) {
    const percentage = (score / total) * 100;
    
    if (percentage >= 90) return "¡Excelente! Eres un experto en República Dominicana 🏆";
    if (percentage >= 70) return "¡Muy bien! Tienes buenos conocimientos 👏";
    if (percentage >= 50) return "¡Bien! Pero puedes mejorar 📚";
    return "Necesitas estudiar más sobre República Dominicana 📖";
}

function getScoreColor(score, total) {
    const percentage = (score / total) * 100;
    
    if (percentage >= 70) return 'success';
    if (percentage >= 50) return 'warning';
    return 'danger';
}


async function fetchQuizQuestions(difficulty = null, provinceId = null) {
    try {
        let url = `${API_BASE_URL}/QuizQuestions`;
        const params = new URLSearchParams();
        
      
        if (difficulty && difficulty !== '') {
            params.append('difficulty', difficulty);
        }
        if (provinceId && provinceId !== '') {
            params.append('provinceId', provinceId);
        }
        
        if (params.toString()) {
            url += `?${params.toString()}`;
        }
        
        console.log('Fetching questions from:', url); // Debug
        
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        const questions = await response.json();
        
        console.log('Questions received:', questions.length); // Debug
        
       
        let filteredQuestions = questions;
        if (difficulty && difficulty !== '') {
            filteredQuestions = questions.filter(q => q.difficultyLevel === difficulty);
        }
        if (provinceId && provinceId !== '') {
            filteredQuestions = filteredQuestions.filter(q => q.provinceId == provinceId);
        }
        
        console.log('Filtered questions:', filteredQuestions.length); // Debug
        
      
        const shuffled = shuffleArray(filteredQuestions);
        const limited = shuffled.slice(0, 10);
        
        console.log('Final questions for quiz:', limited.length); // Debug
        
        return limited;
    } catch (error) {
        console.error('Error al cargar preguntas del quiz:', error);
        showError('No se pudieron cargar las preguntas del quiz: ' + error.message);
        return [];
    }
}

async function updateAttraction(attractionData) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/${attractionData.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(attractionData)
        });
        if (!response.ok) throw new Error('Error al actualizar atracción');
        showSuccess('Atracción actualizada exitosamente');
        await loadAttractionsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo actualizar la atracción: ' + error.message);
        return false;
    }
}

async function deleteAttraction(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error('Error al eliminar atracción');
        showSuccess('Atracción eliminada exitosamente');
        await loadAttractionsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo eliminar la atracción: ' + error.message);
        return false;
    }
}


async function fetchProducts() {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/with-province`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        showError('Error al cargar productos: ' + error.message);
        return [];
    }
}

async function createProduct(productData) {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        });
        if (!response.ok) throw new Error('Error al crear producto');
        showSuccess('Producto creado exitosamente');
        await loadProductsForAdmin();
        return await response.json();
    } catch (error) {
        showError('No se pudo crear el producto: ' + error.message);
        return false;
    }
}

async function updateProduct(productData) {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/${productData.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        });
        if (!response.ok) throw new Error('Error al actualizar producto');
        showSuccess('Producto actualizado exitosamente');
        await loadProductsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo actualizar el producto: ' + error.message);
        return false;
    }
}

async function deleteProduct(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error('Error al eliminar producto');
        showSuccess('Producto eliminado exitosamente');
        await loadProductsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo eliminar el producto: ' + error.message);
        return false;
    }
}


async function loadProvinces() {
    try {
        const data = await fetchProvinces();
        allProvinces = data;
        
        updateGeneralStats();
        populateRegionFilter();
        
        if (data.length > 0) {
            showSuccess(`Se cargaron ${data.length} provincias correctamente`);
            const loadingMessage = document.getElementById('loadingMessage');
            const mapContent = document.getElementById('mapContent');
            
            if (loadingMessage) loadingMessage.style.display = 'none';
            if (mapContent) mapContent.style.display = 'block';
        }
        
        
        await populateQuizProvinceSelect();
        
    } catch (error) {
        console.error('Error loading provinces:', error);
    }
}

function updateGeneralStats() {
    if (allProvinces.length > 0) {
        currentStats.totalProvinces = allProvinces.length;
        currentStats.totalPopulation = allProvinces.reduce((sum, province) => sum + (province.population || 0), 0);
        
        const totalProvincesEl = document.getElementById('totalProvinces');
        const totalPopulationEl = document.getElementById('totalPopulation');
        
        if (totalProvincesEl) totalProvincesEl.textContent = currentStats.totalProvinces;
        if (totalPopulationEl) totalPopulationEl.textContent = formatNumber(currentStats.totalPopulation);
        
        updateFunFacts();
    }
}

function updateFunFacts() {
    if (allProvinces.length === 0) return;
    
    const largestProvince = allProvinces.reduce((prev, current) => 
        (prev.areaKm2 > current.areaKm2) ? prev : current
    );
    
    const mostPopulated = allProvinces.reduce((prev, current) => 
        (prev.population > current.population) ? prev : current
    );
    
    const funFactsContainer = document.getElementById('funFactsContainer');
    if (funFactsContainer) {
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
}


async function loadQuestionsForAdmin() {
    try {
        allQuestions = await fetchQuestions();
        displayQuestionsTable();
    } catch (error) {
        console.error('Error loading questions for admin:', error);
    }
}

function displayQuestionsTable() {
    const tableBody = document.getElementById('questionsTableBody');
    if (!tableBody) return;
    
    tableBody.innerHTML = '';
    
    if (allQuestions.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">No hay preguntas registradas</td></tr>';
        return;
    }
    
    const difficultyFilter = document.getElementById('difficultyFilter')?.value || '';
    const provinceFilter = document.getElementById('provinceFilterQuiz')?.value || '';
    
    let filteredQuestions = allQuestions;
    if (difficultyFilter) {
        filteredQuestions = filteredQuestions.filter(q => q.difficultyLevel === difficultyFilter);
    }
    if (provinceFilter) {
        filteredQuestions = filteredQuestions.filter(q => q.provinceId == provinceFilter);
    }
    
    filteredQuestions.forEach(question => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${question.id}</td>
            <td>
                <div class="question-preview" style="max-width: 200px;">
                    ${question.text.length > 50 ? question.text.substring(0, 50) + '...' : question.text}
                </div>
            </td>
            <td>
                <span class="badge ${
                    question.difficultyLevel === 'Facil' ? 'bg-success' :
                    question.difficultyLevel === 'Intermedio' ? 'bg-warning' : 'bg-danger'
                }">
                    ${question.difficultyLevel}
                </span>
            </td>
            <td>
                <button class="btn btn-primary btn-sm me-1" onclick="selectQuestion(${question.id})" title="Ver respuestas">
                    <i class="fas fa-eye"></i>
                </button>
                <button class="btn btn-warning btn-sm me-1" onclick="editQuestion(${question.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-danger btn-sm" onclick="confirmDeleteQuestion(${question.id})" title="Eliminar">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}

async function loadAnswersForQuestion(questionId) {
    try {
        selectedQuestionId = questionId;
        const answers = await fetchAnswersByQuestionId(questionId);
        allAnswers = answers;
        
        const question = allQuestions.find(q => q.id === questionId);
        const selectedQuestionText = document.getElementById('selectedQuestionText');
        if (selectedQuestionText) {
            selectedQuestionText.textContent = question ? `- ${question.text.substring(0, 30)}...` : '';
        }
        
        const noQuestionSelected = document.getElementById('noQuestionSelected');
        const answersContainer = document.getElementById('answersContainer');
        
        if (noQuestionSelected) noQuestionSelected.style.display = 'none';
        if (answersContainer) answersContainer.style.display = 'block';
        
        displayAnswersTable();
    } catch (error) {
        console.error('Error loading answers:', error);
    }
}

function displayAnswersTable() {
    const tableBody = document.getElementById('answersTableBody');
    if (!tableBody) return;
    
    tableBody.innerHTML = '';
    
    if (allAnswers.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="3" class="text-center text-muted">No hay respuestas</td></tr>';
        return;
    }
    
    allAnswers.forEach(answer => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${answer.text}</td>
            <td>
                <span class="badge ${answer.isCorrect ? 'bg-success' : 'bg-secondary'}">
                    ${answer.isCorrect ? 'Correcta' : 'Incorrecta'}
                </span>
            </td>
            <td>
                <button class="btn btn-warning btn-sm me-1" onclick="editAnswer(${answer.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-danger btn-sm" onclick="confirmDeleteAnswer(${answer.id})" title="Eliminar">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}

async function loadAttractionsForAdmin() {
    try {
        allAttractions = await fetchAttractions();
        displayAttractionsTable();
    } catch (error) {
        console.error('Error loading attractions for admin:', error);
    }
}

function displayAttractionsTable() {
    const tableBody = document.getElementById('attractionsTableBody');
    if (!tableBody) return;
    
    tableBody.innerHTML = '';
    
    if (allAttractions.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No hay atracciones registradas</td></tr>';
        return;
    }
    
    const searchTerm = document.getElementById('attractionSearchInput')?.value.toLowerCase() || '';
    const provinceFilter = document.getElementById('provinceFilterAttractions')?.value || '';
    
    let filteredAttractions = allAttractions;
    if (searchTerm) {
        filteredAttractions = filteredAttractions.filter(a => 
            a.name.toLowerCase().includes(searchTerm) ||
            a.location?.toLowerCase().includes(searchTerm)
        );
    }
    if (provinceFilter) {
        filteredAttractions = filteredAttractions.filter(a => a.provinceId == provinceFilter);
    }
    
    filteredAttractions.forEach(attraction => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${attraction.id}</td>
            <td>${attraction.name}</td>
            <td>${attraction.provinceName || 'Sin provincia'}</td>
            <td>${attraction.location || 'Sin ubicación'}</td>
            <td>
                <button class="btn btn-warning btn-sm me-1" onclick="editAttraction(${attraction.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-danger btn-sm" onclick="confirmDeleteAttraction(${attraction.id})" title="Eliminar">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}

async function loadProductsForAdmin() {
    try {
        allProducts = await fetchProducts();
        displayProductsTable();
    } catch (error) {
        console.error('Error loading products for admin:', error);
    }
}

function displayProductsTable() {
    const tableBody = document.getElementById('productsTableBody');
    if (!tableBody) return;
    
    tableBody.innerHTML = '';
    
    if (allProducts.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No hay productos registrados</td></tr>';
        return;
    }
    
    const searchTerm = document.getElementById('productSearchInput')?.value.toLowerCase() || '';
    const provinceFilter = document.getElementById('provinceFilterProducts')?.value || '';
    
    let filteredProducts = allProducts;
    if (searchTerm) {
        filteredProducts = filteredProducts.filter(p => 
            p.name.toLowerCase().includes(searchTerm) ||
            p.description?.toLowerCase().includes(searchTerm)
        );
    }
    if (provinceFilter) {
        filteredProducts = filteredProducts.filter(p => p.provinceId == provinceFilter);
    }
    
    filteredProducts.forEach(product => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${product.id}</td>
            <td>${product.name}</td>
            <td>${product.provinceName || 'Sin provincia'}</td>
            <td>
                <div style="max-width: 200px;">
                    ${product.description ? 
                        (product.description.length > 50 ? 
                            product.description.substring(0, 50) + '...' : 
                            product.description) 
                        : 'Sin descripción'}
                </div>
            </td>
            <td>
                <button class="btn btn-warning btn-sm me-1" onclick="editProduct(${product.id})" title="Editar">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-danger btn-sm" onclick="confirmDeleteProduct(${product.id})" title="Eliminar">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}


async function selectProvince(provinceId) {
    if (!provinceId) {
        resetProvinceSelection();
        return;
    }
    
    try {
        selectedProvinceId = provinceId;
        let province = allProvinces.find(p => p.id === provinceId);
        
        if (!province) {
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
    const noProvinceSelected = document.getElementById('noProvinceSelected');
    const provinceDetailsContainer = document.getElementById('provinceDetailsContainer');
    
    if (noProvinceSelected) noProvinceSelected.style.display = 'none';
    if (provinceDetailsContainer) provinceDetailsContainer.style.display = 'block';
    
    const elements = {
        provinceName: document.getElementById('provinceName'),
        provinceRegion: document.getElementById('provinceRegion'),
        provinceCapital: document.getElementById('provinceCapital'),
        provincePopulation: document.getElementById('provincePopulation'),
        provinceArea: document.getElementById('provinceArea'),
        provinceDensity: document.getElementById('provinceDensity')
    };
    
    if (elements.provinceName) elements.provinceName.textContent = province.name;
    if (elements.provinceRegion) elements.provinceRegion.textContent = province.region;
    if (elements.provinceCapital) elements.provinceCapital.textContent = province.capital;
    if (elements.provincePopulation) elements.provincePopulation.textContent = formatNumber(province.population);
    if (elements.provinceArea) elements.provinceArea.textContent = `${province.areaKm2.toFixed(1)} km²`;
    if (elements.provinceDensity) elements.provinceDensity.textContent = `${province.density.toFixed(1)} hab/km²`;
    
    await loadProvinceAttractions(province.id);
    await loadProvinceProducts(province.id);
}

async function loadProvinceAttractions(provinceId) {
    const container = document.getElementById('attractionsContainer');
    if (!container) return;
    
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
    if (!container) return;
    
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
    const noProvinceSelected = document.getElementById('noProvinceSelected');
    const provinceDetailsContainer = document.getElementById('provinceDetailsContainer');
    
    if (noProvinceSelected) noProvinceSelected.style.display = 'block';
    if (provinceDetailsContainer) provinceDetailsContainer.style.display = 'none';
}



function openQuestionModal(questionId = null) {
    const modal = new bootstrap.Modal(document.getElementById('questionModal'));
    const form = document.getElementById('questionForm');
    
    if (questionId) {
        const question = allQuestions.find(q => q.id === questionId);
        if (question) {
            document.getElementById('productDescription').value = product.description || '';
            document.getElementById('productProvince').value = product.provinceId;
        }
    } else {
        document.getElementById('productModalTitle').innerHTML = '<i class="fas fa-plus"></i> Agregar Producto';
        form.reset();
        document.getElementById('productId').value = '';
    }
    
    modal.show();
}

async function saveProduct() {
    console.log('Iniciando saveProduct');
    
    const id = document.getElementById('productId').value;
    const name = document.getElementById('productName').value.trim();
    const description = document.getElementById('productDescription').value.trim();
    const provinceId = document.getElementById('productProvince').value;
    
    if (!name || !provinceId) {
        showError('Por favor complete todos los campos requeridos');
        return;
    }
    
    const productData = {
        name: name,
        description: description || null,
        provinceId: parseInt(provinceId)
    };
    
    console.log('Datos a enviar:', productData);
    
    let success = false;
    if (id) {
        productData.id = parseInt(id);
        console.log('Actualizando producto...');
        success = await updateProduct(productData);
    } else {
        console.log('Creando producto...');
        success = await createProduct(productData);
    }
    
    console.log('Resultado:', success);
    
    if (success) {
        console.log('Cerrando modal...');
        
        const modalElement = document.getElementById('productModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        
        if (modal) {
            modal.hide();
        }
        
     
        setTimeout(() => {
            
            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
                backdrop.remove();
            }
            
          
            document.body.classList.remove('modal-open');
            document.body.style.overflow = 'auto';  // Forzar scroll
            document.body.style.paddingRight = '0px';
            document.body.style.position = '';
            
            // Asegurar que el html también tenga scroll
            document.documentElement.style.overflow = 'auto';
            
            console.log('Scroll restaurado');
        }, 100); 
    }
}

function editProduct(productId) {
    openProductModal(productId);
}

function confirmDeleteProduct(productId) {
    const product = allProducts.find(p => p.id === productId);
    
    Swal.fire({
        title: '¿Estás seguro?',
        text: `¿Deseas eliminar el producto "${product ? product.name : 'desconocido'}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            deleteProduct(productId);
        }
    });
}

// ================================
// FUNCIONES DE EVENTOS
// ================================

function setupEventListeners() {
    // Modo Admin/Público
    const toggleAdminBtn = document.getElementById('toggleAdminMode');
    const exitAdminBtn = document.getElementById('exitAdminMode');
    
    if (toggleAdminBtn) {
        toggleAdminBtn.addEventListener('click', toggleAdminMode);
    }
    if (exitAdminBtn) {
        exitAdminBtn.addEventListener('click', toggleAdminMode);
    }

    // Botones modo público
    const refreshDataBtn = document.getElementById('refreshData');
    const viewAllProvincesBtn = document.getElementById('viewAllProvinces');
    const startQuizBtn = document.getElementById('startQuizBtn');
    
    if (refreshDataBtn) {
        refreshDataBtn.addEventListener('click', async function() {
            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Actualizando...';
            this.disabled = true;
            
            await loadProvinces();
            
            this.innerHTML = '<i class="fas fa-sync-alt"></i> Actualizar datos';
            this.disabled = false;
        });
    }
    
    if (viewAllProvincesBtn) {
        viewAllProvincesBtn.addEventListener('click', showProvincesModal);
    }
    
    if (startQuizBtn) {
        startQuizBtn.addEventListener('click', showQuizConfigModal);
    }

    // Botones modo admin - Quiz
    const addQuestionBtn = document.getElementById('addQuestionBtn');
    const saveQuestionBtn = document.getElementById('saveQuestionBtn');
    const addAnswerBtn = document.getElementById('addAnswerBtn');
    const saveAnswerBtn = document.getElementById('saveAnswerBtn');
    
    if (addQuestionBtn) {
        addQuestionBtn.addEventListener('click', () => openQuestionModal());
    }
    if (saveQuestionBtn) {
        saveQuestionBtn.addEventListener('click', saveQuestion);
    }
    if (addAnswerBtn) {
        addAnswerBtn.addEventListener('click', () => openAnswerModal());
    }
    if (saveAnswerBtn) {
        saveAnswerBtn.addEventListener('click', saveAnswer);
    }

    // Botones modo admin - Atracciones
    const addAttractionBtn = document.getElementById('addAttractionBtn');
    const saveAttractionBtn = document.getElementById('saveAttractionBtn');
    
    if (addAttractionBtn) {
        addAttractionBtn.addEventListener('click', () => openAttractionModal());
    }
    if (saveAttractionBtn) {
        saveAttractionBtn.addEventListener('click', saveAttraction);
    }

    // Botones modo admin - Productos
    const addProductBtn = document.getElementById('addProductBtn');
    const saveProductBtn = document.getElementById('saveProductBtn');
    
    if (addProductBtn) {
        addProductBtn.addEventListener('click', () => openProductModal());
    }
    if (saveProductBtn) {
        saveProductBtn.addEventListener('click', saveProduct);
    }
    
   
    setupQuizEventListeners();
    setupCancelButtons();
}


function setupCancelButtons() {
    
    const cancelButtons = [
        { buttonId: 'cancelQuizConfig', modalId: 'quizConfigModal' },
       
    ];
    
  
    const dismissButtons = document.querySelectorAll('[data-bs-dismiss="modal"]');
    
    dismissButtons.forEach(button => {
        button.addEventListener('click', function() {
            setTimeout(() => {
                const backdrop = document.querySelector('.modal-backdrop');
                if (backdrop) {
                    backdrop.remove();
                }
                
             
                document.body.classList.remove('modal-open');
                document.body.style.overflow = 'auto';
                document.body.style.paddingRight = '0px';
                document.body.style.position = '';
                document.documentElement.style.overflow = 'auto';
                
                console.log('Modal cancelado - scroll restaurado');
            }, 150);
        });
    });
}

function setupQuizEventListeners() {
    const startQuizFromConfigBtn = document.getElementById('startQuizFromConfig');
    const cancelQuizConfigBtn = document.getElementById('cancelQuizConfig');
    
    if (startQuizFromConfigBtn) {
        startQuizFromConfigBtn.addEventListener('click', startQuiz);
    }
    if (cancelQuizConfigBtn) {
        cancelQuizConfigBtn.addEventListener('click', function() {
            const modal = bootstrap.Modal.getInstance(document.getElementById('quizConfigModal'));
            if (modal) modal.hide();
        });
    }
    
    // Botones de navegación del quiz
    const nextBtn = document.getElementById('quizNextBtn');
    const prevBtn = document.getElementById('quizPrevBtn');
    const finishBtn = document.getElementById('quizFinishBtn');
    const restartBtn = document.getElementById('restartQuizBtn');
    const exitBtn = document.getElementById('exitQuizBtn');
    
    if (nextBtn) nextBtn.addEventListener('click', nextQuestion);
    if (prevBtn) prevBtn.addEventListener('click', prevQuestion);
    if (finishBtn) finishBtn.addEventListener('click', finishQuiz);
    if (restartBtn) restartBtn.addEventListener('click', restartQuiz);
    if (exitBtn) exitBtn.addEventListener('click', exitQuiz);
}

// ================================
// FUNCIONES DE BÚSQUEDA Y FILTROS
// ================================

function setupSearchFunctionality() {
    const searchInput = document.getElementById('searchInput');
    const modalSearchInput = document.getElementById('modalSearchInput');
    const regionFilter = document.getElementById('regionFilter');
    
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            filterProvinces(searchTerm);
        });
    }
    
    if (modalSearchInput) {
        modalSearchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const regionFilterValue = document.getElementById('regionFilter')?.value || '';
            updateProvincesModal(searchTerm, regionFilterValue);
        });
    }
    
    if (regionFilter) {
        regionFilter.addEventListener('change', function() {
            const searchTerm = document.getElementById('modalSearchInput')?.value.toLowerCase() || '';
            updateProvincesModal(searchTerm, this.value);
        });
    }
    
    setupAdminFilters();
}

// ================================
// FUNCIONES GLOBALES (para HTML)
// ================================

// Hacer funciones disponibles globalmente para onclick en HTML
window.selectProvinceFromModal = selectProvinceFromModal;
window.selectProvince = selectProvince;
window.selectQuestion = selectQuestion;
window.editQuestion = editQuestion;
window.confirmDeleteQuestion = confirmDeleteQuestion;
window.editAnswer = editAnswer;
window.confirmDeleteAnswer = confirmDeleteAnswer;
window.editAttraction = editAttraction;
window.confirmDeleteAttraction = confirmDeleteAttraction;
window.editProduct = editProduct;
window.confirmDeleteProduct = confirmDeleteProduct;

// Funciones del quiz
window.showQuizConfigModal = showQuizConfigModal;
window.startQuiz = startQuiz;
window.nextQuestion = nextQuestion;
window.prevQuestion = prevQuestion;
window.finishQuiz = finishQuiz;
window.restartQuiz = restartQuiz;
window.exitQuiz = exitQuiz;

// ================================
// INICIALIZACIÓN
// ================================

async function initializeApp() {
    try {
        console.log('Iniciando Atlas Interactivo...');
        
        // Configurar event listeners
        setupEventListeners();
        setupSearchFunctionality();
        
        // Cargar datos iniciales del modo público
        await loadProvinces();
        
        // Si Google Maps ya está cargado y hay provincias, inicializar marcadores
        if (typeof google !== 'undefined' && window.map && allProvinces.length > 0) {
            console.log('Inicializando marcadores de Google Maps...');
            updateMapMarkers();
        }
        
        console.log('Atlas Dominicano inicializado correctamente');
        
    } catch (error) {
        console.error('Error al inicializar la aplicación:', error);
        showError('Error al inicializar la aplicación. Por favor, recarga la página.');
    }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

function openQuestionModal(questionId = null) {
    const modal = new bootstrap.Modal(document.getElementById('questionModal'));
    const form = document.getElementById('questionForm');
    
    if (questionId) {
        const question = allQuestions.find(q => q.id === questionId);
        if (question) {
            document.getElementById('questionModalTitle').innerHTML = '<i class="fas fa-edit"></i> Editar Pregunta';
            document.getElementById('questionId').value = question.id;
            document.getElementById('questionText').value = question.text;
            document.getElementById('questionDifficulty').value = question.difficultyLevel;
            document.getElementById('questionProvince').value = question.provinceId || '';
        }
    } else {
        document.getElementById('questionModalTitle').innerHTML = '<i class="fas fa-plus"></i> Agregar Pregunta';
        form.reset();
        document.getElementById('questionId').value = '';
    }
    
    modal.show();
}

async function saveQuestion() {
    const id = document.getElementById('questionId').value;
    const text = document.getElementById('questionText').value.trim();
    const difficultyLevel = document.getElementById('questionDifficulty').value;
    const provinceId = document.getElementById('questionProvince').value || null;
    
    if (!text || !difficultyLevel) {
        showError('Por favor complete todos los campos requeridos');
        return;
    }
    
    const questionData = {
        text: text,
        difficultyLevel: difficultyLevel,
        provinceId: provinceId ? parseInt(provinceId) : null
    };
    
    let success = false;
    if (id) {
        questionData.id = parseInt(id);
        success = await updateQuestion(questionData);
    } else {
        success = await createQuestion(questionData);
    }
    
    if (success) {
        const modalElement = document.getElementById('questionModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        
        if (modal) {
            modal.hide();
        }
        
        setTimeout(() => {
            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
                backdrop.remove();
            }
            
            document.body.classList.remove('modal-open');
            document.body.style.overflow = 'auto';
            document.body.style.paddingRight = '0px';
            document.body.style.position = '';
            
            document.documentElement.style.overflow = 'auto';
        }, 100);
    }
}

function selectQuestion(questionId) {
    document.querySelectorAll('#questionsTableBody tr').forEach(row => {
        row.classList.remove('table-primary');
    });
    
    const selectedRow = document.querySelector(`#questionsTableBody tr:has(button[onclick="selectQuestion(${questionId})"])`);
    if (selectedRow) {
        selectedRow.classList.add('table-primary');
    }
    
    loadAnswersForQuestion(questionId);
}

function editQuestion(questionId) {
    openQuestionModal(questionId);
}

function confirmDeleteQuestion(questionId) {
    const question = allQuestions.find(q => q.id === questionId);
    
    Swal.fire({
        title: '¿Estás seguro?',
        text: `¿Deseas eliminar la pregunta "${question ? question.text.substring(0, 50) : 'desconocida'}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            deleteQuestion(questionId);
        }
    });
}

function openAnswerModal(answerId = null) {
    if (!selectedQuestionId) {
        showError('Primero selecciona una pregunta');
        return;
    }
    
    const modal = new bootstrap.Modal(document.getElementById('answerModal'));
    const form = document.getElementById('answerForm');
    
    if (answerId) {
        const answer = allAnswers.find(a => a.id === answerId);
        if (answer) {
            document.getElementById('answerModalTitle').innerHTML = '<i class="fas fa-edit"></i> Editar Respuesta';
            document.getElementById('answerId').value = answer.id;
            document.getElementById('answerText').value = answer.text;
            document.getElementById('answerIsCorrect').checked = answer.isCorrect;
            document.getElementById('answerQuestionId').value = answer.questionId;
        }
    } else {
        document.getElementById('answerModalTitle').innerHTML = '<i class="fas fa-plus"></i> Agregar Respuesta';
        form.reset();
        document.getElementById('answerId').value = '';
        document.getElementById('answerQuestionId').value = selectedQuestionId;
    }
    
    modal.show();
}

async function saveAnswer() {
    const id = document.getElementById('answerId').value;
    const text = document.getElementById('answerText').value.trim();
    const isCorrect = document.getElementById('answerIsCorrect').checked;
    const questionId = document.getElementById('answerQuestionId').value;
    
    if (!text) {
        showError('Por favor complete el texto de la respuesta');
        return;
    }
    
    const answerData = {
        text: text,
        isCorrect: isCorrect,
        questionId: parseInt(questionId)
    };
    
    let success = false;
    if (id) {
        answerData.id = parseInt(id);
        success = await updateAnswer(answerData);
    } else {
        success = await createAnswer(answerData);
    }
    
    if (success) {
        const modalElement = document.getElementById('answerModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        
        if (modal) {
            modal.hide();
        }
        
        setTimeout(() => {
            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
                backdrop.remove();
            }
            
            document.body.classList.remove('modal-open');
            document.body.style.overflow = 'auto';
            document.body.style.paddingRight = '0px';
            document.body.style.position = '';
            
            document.documentElement.style.overflow = 'auto';
        }, 100);
    }
}

function editAnswer(answerId) {
    openAnswerModal(answerId);
}

function confirmDeleteAnswer(answerId) {
    const answer = allAnswers.find(a => a.id === answerId);
    
    Swal.fire({
        title: '¿Estás seguro?',
        text: `¿Deseas eliminar la respuesta "${answer ? answer.text : 'desconocida'}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            deleteAnswer(answerId, selectedQuestionId);
        }
    });
}

// ================================
// FUNCIONES DE MODAL - ADMIN ATRACCIONES
// ================================

function openAttractionModal(attractionId = null) {
    const modal = new bootstrap.Modal(document.getElementById('attractionModal'));
    const form = document.getElementById('attractionForm');
    
    if (attractionId) {
        const attraction = allAttractions.find(a => a.id === attractionId);
        if (attraction) {
            document.getElementById('attractionModalTitle').innerHTML = '<i class="fas fa-edit"></i> Editar Atracción';
            document.getElementById('attractionId').value = attraction.id;
            document.getElementById('attractionName').value = attraction.name;
            document.getElementById('attractionLocation').value = attraction.location || '';
            document.getElementById('attractionDescription').value = attraction.description || '';
            document.getElementById('attractionProvince').value = attraction.provinceId;
        }
    } else {
        document.getElementById('attractionModalTitle').innerHTML = '<i class="fas fa-plus"></i> Agregar Atracción';
        form.reset();
        document.getElementById('attractionId').value = '';
    }
    
    modal.show();
}

async function saveAttraction() {
    const id = document.getElementById('attractionId').value;
    const name = document.getElementById('attractionName').value.trim();
    const location = document.getElementById('attractionLocation').value.trim();
    const description = document.getElementById('attractionDescription').value.trim();
    const provinceId = document.getElementById('attractionProvince').value;
    
    if (!name || !provinceId) {
        showError('Por favor complete todos los campos requeridos');
        return;
    }
    
    const attractionData = {
        name: name,
        location: location || null,
        description: description || null,
        provinceId: parseInt(provinceId)
    };
    
    let success = false;
    if (id) {
        attractionData.id = parseInt(id);
        success = await updateAttraction(attractionData);
    } else {
        success = await createAttraction(attractionData);
    }
    
    if (success) {
        const modalElement = document.getElementById('attractionModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        
        if (modal) {
            modal.hide();
        }
        
        setTimeout(() => {
            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
                backdrop.remove();
            }
            
            document.body.classList.remove('modal-open');
            document.body.style.overflow = 'auto';
            document.body.style.paddingRight = '0px';
            document.body.style.position = '';
            
            document.documentElement.style.overflow = 'auto';
        }, 100);
    }
}

function editAttraction(attractionId) {
    openAttractionModal(attractionId);
}

function confirmDeleteAttraction(attractionId) {
    const attraction = allAttractions.find(a => a.id === attractionId);
    
    Swal.fire({
        title: '¿Estás seguro?',
        text: `¿Deseas eliminar la atracción "${attraction ? attraction.name : 'desconocida'}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            deleteAttraction(attractionId);
        }
    });
}

// ================================
// FUNCIONES DE MODAL - ADMIN PRODUCTOS
// ================================

function openProductModal(productId = null) {
    const modal = new bootstrap.Modal(document.getElementById('productModal'));
    const form = document.getElementById('productForm');
    
    if (productId) {
        const product = allProducts.find(p => p.id === productId);
        if (product) {
            document.getElementById('productModalTitle').innerHTML = '<i class="fas fa-edit"></i> Editar Producto';
            document.getElementById('productId').value = product.id;
            document.getElementById('productName').value = product.name;
            document.getElementById('productDescription').value = product.description || '';
            document.getElementById('productProvince').value = product.provinceId;
        }
    } else {
        document.getElementById('productModalTitle').innerHTML = '<i class="fas fa-plus"></i> Agregar Producto';
        form.reset();
        document.getElementById('productId').value = '';
    }
    
    modal.show();
}

async function fetchQuestionAnswers(questionId) {
    try {
        console.log('Fetching answers for question:', questionId); // Debug
        
        const response = await fetch(`${API_BASE_URL}/quizanswers/by-question?questionId=${questionId}`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        const answers = await response.json();
        
        console.log('Answers received for question', questionId, ':', answers.length); // Debug
        
        const hasCorrectAnswer = answers.some(answer => answer.isCorrect);
        if (!hasCorrectAnswer) {
            console.warn('Question', questionId, 'has no correct answers');
        }
        
        return shuffleArray(answers);
    } catch (error) {
        console.error('Error loading answers for question', questionId, ':', error);
        return [];
    }
}

function showQuizConfigModal() {
    const modal = new bootstrap.Modal(document.getElementById('quizConfigModal'));
    modal.show();
}

async function startQuiz() {
    const difficulty = document.getElementById('quizDifficulty').value;
    const provinceId = document.getElementById('quizProvince').value || null;
    
    console.log('Starting quiz with:', { difficulty, provinceId }); // Debug
    
    const configModal = bootstrap.Modal.getInstance(document.getElementById('quizConfigModal'));
    if (configModal) configModal.hide();
    
    showQuizModal();
    showQuizLoading();
    
    try {
        const questions = await fetchQuizQuestions(difficulty, provinceId);
        
        if (questions.length === 0) {
            hideQuizLoading();
            showQuizError('No se encontraron preguntas para los criterios seleccionados. Intenta con otros filtros.');
            return;
        }
        
        console.log('Loading answers for', questions.length, 'questions'); // Debug
        
        let validQuestions = [];
        for (let question of questions) {
            const answers = await fetchQuestionAnswers(question.id);
            if (answers && answers.length > 0) {
                question.answers = answers;
                validQuestions.push(question);
            }
        }
        
        console.log('Valid questions with answers:', validQuestions.length); // Debug
        
        if (validQuestions.length === 0) {
            hideQuizLoading();
            showQuizError('No se encontraron preguntas válidas con respuestas. Contacta al administrador.');
            return;
        }
        
        quizData = {
            questions: validQuestions,
            currentQuestionIndex: 0,
            userAnswers: [],
            score: 0,
            isActive: true,
            difficulty: difficulty,
            provinceId: provinceId
        };
        
        hideQuizLoading();
        displayCurrentQuestion();
        
    } catch (error) {
        console.error('Error starting quiz:', error); // Debug
        hideQuizLoading();
        showQuizError('Error al iniciar el quiz: ' + error.message);
    }
}

function showQuizModal() {
    const modal = new bootstrap.Modal(document.getElementById('quizModal'), {
        backdrop: 'static',
        keyboard: false
    });
    modal.show();
}

function displayCurrentQuestion() {
    const question = quizData.questions[quizData.currentQuestionIndex];
    
    console.log('Displaying question:', question); // Debug
    console.log('Question text:', question?.text); // Debug
    
    if (!question) {
        showQuizError('Error: No se pudo cargar la pregunta.');
        return;
    }
    
    document.getElementById('quizContent').innerHTML = `
        <div class="question-container mb-4">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <h6 class="question-title mb-0">Pregunta:</h6>
                <span id="questionDifficulty" class="badge ${
                    question.difficultyLevel === 'Facil' ? 'bg-success' :
                    question.difficultyLevel === 'Intermedio' ? 'bg-warning' : 'bg-danger'
                }">${question.difficultyLevel}</span>
            </div>
            <p id="questionText" class="lead">
                ${question.text || 'Pregunta no disponible'}
            </p>
        </div>
        
        <div class="answers-container">
            <h6 class="mb-3">Selecciona tu respuesta:</h6>
            <div id="quizAnswers">
                <!-- Respuestas dinámicas -->
            </div>
        </div>
    `;
    
    // Mostrar footer
    document.getElementById('quizContent-footer').style.display = 'flex';
    
    // Actualizar información del quiz
    document.getElementById('quizProgress').textContent = 
        `Pregunta ${quizData.currentQuestionIndex + 1} de ${quizData.questions.length}`;
    
    document.getElementById('quizScore').textContent = 
        `Puntuación: ${quizData.score}/${quizData.currentQuestionIndex}`;
    
    // Actualizar barra de progreso
    const progressPercentage = ((quizData.currentQuestionIndex + 1) / quizData.questions.length) * 100;
    document.getElementById('quizProgressBar').style.width = `${progressPercentage}%`;
    
    // Verificar que hay respuestas
    if (!question.answers || question.answers.length === 0) {
        showQuizError('Error: Esta pregunta no tiene respuestas disponibles.');
        return;
    }
    
    // Mostrar respuestas
    const answersContainer = document.getElementById('quizAnswers');
    if (answersContainer) {
        answersContainer.innerHTML = '';
        
        question.answers.forEach((answer, index) => {
            const answerButton = document.createElement('button');
            answerButton.className = 'btn btn-outline-primary quiz-answer-btn mb-2 w-100 text-start';
            answerButton.innerHTML = `
                <div class="d-flex align-items-center">
                    <span class="answer-letter me-3">${String.fromCharCode(65 + index)}</span>
                    <span class="answer-text">${answer.text}</span>
                </div>
            `;
            answerButton.onclick = () => selectAnswer(answer.id, answer.isCorrect);
            answersContainer.appendChild(answerButton);
        });
    }
    
    // Controles del quiz
    const prevBtn = document.getElementById('quizPrevBtn');
    const nextBtn = document.getElementById('quizNextBtn');
    const finishBtn = document.getElementById('quizFinishBtn');
    
    if (prevBtn) prevBtn.disabled = quizData.currentQuestionIndex === 0;
    if (nextBtn) nextBtn.style.display = 'none';
    if (finishBtn) finishBtn.style.display = 'none';
}

function selectAnswer(answerId, isCorrect) {
    const answerButtons = document.querySelectorAll('.quiz-answer-btn');
    answerButtons.forEach(btn => {
        btn.disabled = true;
        btn.onclick = null;
    });
    
    const question = quizData.questions[quizData.currentQuestionIndex];
    question.answers.forEach((answer, index) => {
        const button = answerButtons[index];
        
        if (answer.id === answerId) {
            button.classList.remove('btn-outline-primary');
            button.classList.add(isCorrect ? 'btn-success' : 'btn-danger');
        } else if (answer.isCorrect) {
            button.classList.remove('btn-outline-primary');
            button.classList.add('btn-success');
        }
    });
    
    quizData.userAnswers[quizData.currentQuestionIndex] = {
        questionId: question.id,
        answerId: answerId,
        isCorrect: isCorrect
    };
    
    if (isCorrect) {
        quizData.score++;
    }
    
    if (quizData.currentQuestionIndex < quizData.questions.length - 1) {
        document.getElementById('quizNextBtn').style.display = 'inline-block';
    } else {
        document.getElementById('quizFinishBtn').style.display = 'inline-block';
    }
}

function nextQuestion() {
    if (quizData.currentQuestionIndex < quizData.questions.length - 1) {
        quizData.currentQuestionIndex++;
        displayCurrentQuestion();
    }
}

function prevQuestion() {
    if (quizData.currentQuestionIndex > 0) {
        quizData.currentQuestionIndex--;
        displayCurrentQuestion();
        
        const userAnswer = quizData.userAnswers[quizData.currentQuestionIndex];
        if (userAnswer) {
            const answerButtons = document.querySelectorAll('.quiz-answer-btn');
            const question = quizData.questions[quizData.currentQuestionIndex];
            
            answerButtons.forEach(btn => btn.disabled = true);
            
            question.answers.forEach((answer, index) => {
                const button = answerButtons[index];
                
                if (answer.id === userAnswer.answerId) {
                    button.classList.remove('btn-outline-primary');
                    button.classList.add(userAnswer.isCorrect ? 'btn-success' : 'btn-danger');
                } else if (answer.isCorrect) {
                    button.classList.remove('btn-outline-primary');
                    button.classList.add('btn-success');
                }
            });
            
            if (quizData.currentQuestionIndex < quizData.questions.length - 1) {
                document.getElementById('quizNextBtn').style.display = 'inline-block';
            } else {
                document.getElementById('quizFinishBtn').style.display = 'inline-block';
            }
        }
    }
}

function finishQuiz() {
    quizData.isActive = false;
    showQuizResults();
}

function showQuizResults() {
    const total = quizData.questions.length;
    const score = quizData.score;
    const percentage = Math.round((score / total) * 100);
    
    document.getElementById('quizContent').style.display = 'none';
    document.getElementById('quizResults').style.display = 'block';
    document.getElementById('quizContent-footer').style.display = 'none';
    document.getElementById('quizResults-footer').style.display = 'flex';
    
    document.getElementById('finalScore').textContent = `${score}/${total}`;
    document.getElementById('finalPercentage').textContent = `${percentage}%`;
    document.getElementById('scoreMessage').textContent = getScoreMessage(score, total);
    
    const scoreColor = getScoreColor(score, total);
    const scoreCard = document.getElementById('scoreCard');
    scoreCard.className = `card border-${scoreColor}`;
    
    const correctAnswers = quizData.userAnswers.filter(answer => answer.isCorrect).length;
    const incorrectAnswers = total - correctAnswers;
    
    document.getElementById('correctAnswers').textContent = correctAnswers;
    document.getElementById('incorrectAnswers').textContent = incorrectAnswers;
    document.getElementById('totalQuestions').textContent = total;
    
    displayIncorrectAnswers();
}

function displayIncorrectAnswers() {
    const incorrectContainer = document.getElementById('incorrectAnswersContainer');
    const incorrectAnswers = quizData.userAnswers.filter(userAnswer => !userAnswer.isCorrect);
    
    if (incorrectAnswers.length === 0) {
        incorrectContainer.innerHTML = '<p class="text-success"><i class="fas fa-trophy"></i> ¡Perfecto! No tuviste respuestas incorrectas.</p>';
        return;
    }
    
    let html = '<h6><i class="fas fa-times-circle text-danger"></i> Respuestas Incorrectas:</h6>';
    
    incorrectAnswers.forEach(userAnswer => {
        const question = quizData.questions.find(q => q.id === userAnswer.questionId);
        const correctAnswer = question.answers.find(a => a.isCorrect);
        const selectedAnswer = question.answers.find(a => a.id === userAnswer.answerId);
        
        html += `
            <div class="incorrect-answer-item mb-3 p-3 border-start border-danger border-3">
                <p class="fw-bold mb-2">${question.text}</p>
                <p class="mb-1 text-danger"><i class="fas fa-times"></i> Tu respuesta: ${selectedAnswer.text}</p>
                <p class="mb-0 text-success"><i class="fas fa-check"></i> Respuesta correcta: ${correctAnswer.text}</p>
            </div>
        `;
    });
    
    incorrectContainer.innerHTML = html;
}

function restartQuiz() {
    document.getElementById('quizContent').style.display = 'block';
    document.getElementById('quizResults').style.display = 'none';
    document.getElementById('quizContent-footer').style.display = 'flex';
    document.getElementById('quizResults-footer').style.display = 'none';
    
    const quizModal = bootstrap.Modal.getInstance(document.getElementById('quizModal'));
    if (quizModal) quizModal.hide();
    
    setTimeout(() => {
        showQuizConfigModal();
    }, 300);
}

function exitQuiz() {
    const quizModal = bootstrap.Modal.getInstance(document.getElementById('quizModal'));
    if (quizModal) quizModal.hide();
    
    quizData = {
        questions: [],
        currentQuestionIndex: 0,
        userAnswers: [],
        score: 0,
        isActive: false,
        difficulty: 'Facil',
        provinceId: null
    };
    
    document.getElementById('quizContent').style.display = 'block';
    document.getElementById('quizResults').style.display = 'none';
    document.getElementById('quizContent-footer').style.display = 'flex';
    document.getElementById('quizResults-footer').style.display = 'none';
}

async function populateQuizProvinceSelect() {
    const select = document.getElementById('quizProvince');
    if (select && allProvinces.length > 0) {
        select.innerHTML = '<option value="">Todas las provincias</option>';
        allProvinces.forEach(province => {
            const option = document.createElement('option');
            option.value = province.id;
            option.textContent = province.name;
            select.appendChild(option);
        });
    }
}


function toggleAdminMode() {
    isAdminMode = !isAdminMode;
    
    const publicMode = document.getElementById('publicMode');
    const adminMode = document.getElementById('adminMode');
    const toggleBtn = document.getElementById('toggleAdminMode');
    
    if (!publicMode || !adminMode || !toggleBtn) {
        console.error('Elementos de modo admin/público no encontrados');
        return;
    }
    
    if (isAdminMode) {
        publicMode.style.display = 'none';
        adminMode.style.display = 'block';
        toggleBtn.innerHTML = '<i class="fas fa-eye"></i> Público';
        toggleBtn.className = 'btn btn-primary btn-sm';
        
        loadAdminData();
        showSuccess('Modo administrador activado');
    } else {
        publicMode.style.display = 'block';
        adminMode.style.display = 'none';
        toggleBtn.innerHTML = '<i class="fas fa-tools"></i> Admin';
        toggleBtn.className = 'btn btn-warning btn-sm';
        
        showSuccess('Volviendo al modo público');
    }
}

async function loadAdminData() {
    try {
        await Promise.all([
            loadQuestionsForAdmin(),
            loadAttractionsForAdmin(),
            loadProductsForAdmin(),
            populateProvinceSelects()
        ]);
    } catch (error) {
        console.error('Error loading admin data:', error);
    }
}

function setupAdminFilters() {
    const difficultyFilter = document.getElementById('difficultyFilter');
    const provinceFilterQuiz = document.getElementById('provinceFilterQuiz');
    
    if (difficultyFilter) {
        difficultyFilter.addEventListener('change', () => displayQuestionsTable());
    }
    if (provinceFilterQuiz) {
        provinceFilterQuiz.addEventListener('change', () => displayQuestionsTable());
    }

    const attractionSearchInput = document.getElementById('attractionSearchInput');
    const provinceFilterAttractions = document.getElementById('provinceFilterAttractions');
    
    if (attractionSearchInput) {
        attractionSearchInput.addEventListener('input', () => displayAttractionsTable());
    }
    if (provinceFilterAttractions) {
        provinceFilterAttractions.addEventListener('change', () => displayAttractionsTable());
    }

    const productSearchInput = document.getElementById('productSearchInput');
    const provinceFilterProducts = document.getElementById('provinceFilterProducts');
    
    if (productSearchInput) {
        productSearchInput.addEventListener('input', () => displayProductsTable());
    }
    if (provinceFilterProducts) {
        provinceFilterProducts.addEventListener('change', () => displayProductsTable());
    }
}

function filterProvinces(searchTerm) {
    const filteredProvinces = allProvinces.filter(province =>
        province.name.toLowerCase().includes(searchTerm) ||
        province.capital.toLowerCase().includes(searchTerm) ||
        province.region.toLowerCase().includes(searchTerm)
    );
    
    console.log(`Encontradas ${filteredProvinces.length} provincias que coinciden con "${searchTerm}"`);
}

function populateRegionFilter() {
    const regionFilter = document.getElementById('regionFilter');
    if (!regionFilter) return;
    
    const regions = [...new Set(allProvinces.map(p => p.region))].sort();
    
    regionFilter.innerHTML = '<option value="">Todas las regiones</option>';
    regions.forEach(region => {
        const option = document.createElement('option');
        option.value = region;
        option.textContent = region;
        regionFilter.appendChild(option);
    });
}

async function populateProvinceSelects() {
    const selects = [
        'questionProvince',
        'provinceFilterQuiz',
        'attractionProvince',
        'provinceFilterAttractions',
        'productProvince',
        'provinceFilterProducts'
    ];
    
    selects.forEach(selectId => {
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = selectId.includes('Filter') ? 
                '<option value="">Todas las provincias</option>' : 
                '<option value="">Selecciona una provincia</option>';
                
            allProvinces.forEach(province => {
                const option = document.createElement('option');
                option.value = province.id;
                option.textContent = province.name;
                select.appendChild(option);
            });
        }
    });
}

function showProvincesModal() {
    updateProvincesModal();
    const modal = new bootstrap.Modal(document.getElementById('provincesModal'));
    modal.show();
}

function updateProvincesModal(searchTerm = '', regionFilter = '') {
    const tableBody = document.getElementById('provincesTableBody');
    if (!tableBody) return;
    
    let filteredProvinces = allProvinces;
    
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
    const modal = bootstrap.Modal.getInstance(document.getElementById('provincesModal'));
    if (modal) modal.hide();
    selectProvince(provinceId);
}

// ================================
// FUNCIONES DE API - PÚBLICAS
// ================================

async function fetchProvinces() {
    try {
        showLoading();
        const response = await fetch(`${API_BASE_URL}/provinces`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
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
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        showError('Error al cargar la provincia: ' + error.message);
        return null;
    }
}

async function fetchTouristAttractionsByProvince(provinceId) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/by-province?provinceId=${provinceId}`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        console.error('Error al cargar atracciones:', error);
        return [];
    }
}

async function fetchTypicalProductsByProvince(provinceId) {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/by-province?provinceId=${provinceId}`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        console.error('Error al cargar productos típicos:', error);
        return [];
    }
}

// ================================
// FUNCIONES DE API - QUIZ ADMIN
// ================================

async function fetchQuestions() {
    try {
        const response = await fetch(`${API_BASE_URL}/quizquestions/with-province`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        showError('Error al cargar preguntas: ' + error.message);
        return [];
    }
}

async function fetchAnswersByQuestionId(questionId) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizanswers/by-question?questionId=${questionId}`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        showError('Error al cargar respuestas: ' + error.message);
        return [];
    }
}

async function createQuestion(questionData) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizquestions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(questionData)
        });
        if (!response.ok) throw new Error('Error al crear pregunta');
        const result = await response.json();
        showSuccess('Pregunta creada exitosamente');
        await loadQuestionsForAdmin();
        return result;
    } catch (error) {
        showError('No se pudo crear la pregunta: ' + error.message);
        return false;
    }
}

async function updateQuestion(questionData) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizquestions/${questionData.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(questionData)
        });
        if (!response.ok) throw new Error('Error al actualizar pregunta');
        showSuccess('Pregunta actualizada exitosamente');
        await loadQuestionsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo actualizar la pregunta: ' + error.message);
        return false;
    }
}

async function deleteQuestion(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizquestions/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error('Error al eliminar pregunta');
        showSuccess('Pregunta eliminada exitosamente');
        await loadQuestionsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo eliminar la pregunta: ' + error.message);
        return false;
    }
}

async function createAnswer(answerData) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizanswers`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(answerData)
        });
        if (!response.ok) throw new Error('Error al crear respuesta');
        showSuccess('Respuesta creada exitosamente');
        await loadAnswersForQuestion(answerData.questionId);
        return await response.json();
    } catch (error) {
        showError('No se pudo crear la respuesta: ' + error.message);
        return false;
    }
}

async function updateAnswer(answerData) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizanswers/${answerData.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(answerData)
        });
        if (!response.ok) throw new Error('Error al actualizar respuesta');
        showSuccess('Respuesta actualizada exitosamente');
        await loadAnswersForQuestion(answerData.questionId);
        return true;
    } catch (error) {
        showError('No se pudo actualizar la respuesta: ' + error.message);
        return false;
    }
}

async function deleteAnswer(id, questionId) {
    try {
        const response = await fetch(`${API_BASE_URL}/quizanswers/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error('Error al eliminar respuesta');
        showSuccess('Respuesta eliminada exitosamente');
        await loadAnswersForQuestion(questionId);
        return true;
    } catch (error) {
        showError('No se pudo eliminar la respuesta: ' + error.message);
        return false;
    }
}

// ================================
// FUNCIONES DE API - ATRACCIONES ADMIN
// ================================

async function fetchAttractions() {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/with-province`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        showError('Error al cargar atracciones: ' + error.message);
        return [];
    }
}

async function createAttraction(attractionData) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(attractionData)
        });
        if (!response.ok) throw new Error('Error al crear atracción');
        showSuccess('Atracción creada exitosamente');
        await loadAttractionsForAdmin();
        return await response.json();
    } catch (error) {
        showError('No se pudo crear la atracción: ' + error.message);
        return false;
    }
}

async function updateAttraction(attractionData) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/${attractionData.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(attractionData)
        });
        if (!response.ok) throw new Error('Error al actualizar atracción');
        showSuccess('Atracción actualizada exitosamente');
        await loadAttractionsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo actualizar la atracción: ' + error.message);
        return false;
    }
}

async function deleteAttraction(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/touristattractions/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error('Error al eliminar atracción');
        showSuccess('Atracción eliminada exitosamente');
        await loadAttractionsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo eliminar la atracción: ' + error.message);
        return false;
    }
}

// ================================
// FUNCIONES DE API - PRODUCTOS ADMIN
// ================================

async function fetchProducts() {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/with-province`);
        if (!response.ok) throw new Error(`Error ${response.status}: ${response.statusText}`);
        return await response.json();
    } catch (error) {
        showError('Error al cargar productos: ' + error.message);
        return [];
    }
}

async function createProduct(productData) {
    try {
        console.log('🔵 Enviando POST...'); // Debug
        const response = await fetch(`${API_BASE_URL}/typicalproducts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        });
        if (!response.ok) throw new Error('Error al crear producto');
        
        console.log('POST exitoso, mostrando mensaje...'); // Debug
        showSuccess('Producto creado exitosamente');
        
        console.log('Recargando tabla...'); // Debug
        await loadProductsForAdmin();
        console.log('Tabla recargada'); // Debug
        
        return await response.json();
    } catch (error) {
        console.error('Error en createProduct:', error); // Debug
        showError('No se pudo crear el producto: ' + error.message);
        return false;
    }
}

async function updateProduct(productData) {
    try {
        const response = await fetch(`${API_BASE_URL}/typicalproducts/${productData.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        });
        if (!response.ok) throw new Error('Error al actualizar producto');
        showSuccess('Producto actualizado exitosamente');
        await loadProductsForAdmin();
        return true;
    } catch (error) {
        showError('No se pudo actualizar el producto: ' + error.message);
        return false;
    }
}



async function loadProvinces() {
    try {
        const data = await fetchProvinces();
        allProvinces = data;
        
        updateGeneralStats();
        populateRegionFilter();
        
        if (data.length > 0) {
            showSuccess(`Se cargaron ${data.length} provincias correctamente`);
            const loadingMessage = document.getElementById('loadingMessage');
            const mapContent = document.getElementById('mapContent');
            
            if (loadingMessage) loadingMessage.style.display = 'none';
            if (mapContent) mapContent.style.display = 'block';
        }
        
        // Poblar select del quiz
        await populateQuizProvinceSelect();
        
        // Actualizar marcadores del mapa si Google Maps está disponible
        if (typeof google !== 'undefined' && window.map) {
            console.log('🗺️ Actualizando marcadores del mapa...');
            updateMapMarkers();
        }
        
    } catch (error) {
        console.error('Error loading provinces:', error);
    }
}

function updateGeneralStats() {
    if (allProvinces.length > 0) {
        currentStats.totalProvinces = allProvinces.length;
        currentStats.totalPopulation = allProvinces.reduce((sum, province) => sum + (province.population || 0), 0);
        
        const totalProvincesEl = document.getElementById('totalProvinces');
        const totalPopulationEl = document.getElementById('totalPopulation');
        
        if (totalProvincesEl) totalProvincesEl.textContent = currentStats.totalProvinces;
        if (totalPopulationEl) totalPopulationEl.textContent = formatNumber(currentStats.totalPopulation);
        
        updateFunFacts();
    }
}

function updateFunFacts() {
    if (allProvinces.length === 0) return;
    
    const largestProvince = allProvinces.reduce((prev, current) => 
        (prev.areaKm2 > current.areaKm2) ? prev : current
    );
    
    const mostPopulated = allProvinces.reduce((prev, current) => 
        (prev.population > current.population) ? prev : current
    );
    
    const funFactsContainer = document.getElementById('funFactsContainer');
    if (funFactsContainer) {
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
}

// ================================
// FUNCIONES DE SELECCIÓN - MODO PÚBLICO
// ================================

async function selectProvince(provinceId) {
    if (!provinceId) {
        resetProvinceSelection();
        return;
    }
    
    try {
        selectedProvinceId = provinceId;
        let province = allProvinces.find(p => p.id === provinceId);
        
        if (!province) {
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
    const noProvinceSelected = document.getElementById('noProvinceSelected');
    const provinceDetailsContainer = document.getElementById('provinceDetailsContainer');
    
    if (noProvinceSelected) noProvinceSelected.style.display = 'none';
    if (provinceDetailsContainer) provinceDetailsContainer.style.display = 'block';
    
    const elements = {
        provinceName: document.getElementById('provinceName'),
        provinceRegion: document.getElementById('provinceRegion'),
        provinceCapital: document.getElementById('provinceCapital'),
        provincePopulation: document.getElementById('provincePopulation'),
        provinceArea: document.getElementById('provinceArea'),
        provinceDensity: document.getElementById('provinceDensity')
    };
    
    if (elements.provinceName) elements.provinceName.textContent = province.name;
    if (elements.provinceRegion) elements.provinceRegion.textContent = province.region;
    if (elements.provinceCapital) elements.provinceCapital.textContent = province.capital;
    if (elements.provincePopulation) elements.provincePopulation.textContent = formatNumber(province.population);
    if (elements.provinceArea) elements.provinceArea.textContent = `${province.areaKm2.toFixed(1)} km²`;
    if (elements.provinceDensity) elements.provinceDensity.textContent = `${province.density.toFixed(1)} hab/km²`;
    
    await loadProvinceAttractions(province.id);
    await loadProvinceProducts(province.id);
}

async function loadProvinceAttractions(provinceId) {
    const container = document.getElementById('attractionsContainer');
    if (!container) return;
    
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
    if (!container) return;
    
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
    const noProvinceSelected = document.getElementById('noProvinceSelected');
    const provinceDetailsContainer = document.getElementById('provinceDetailsContainer');
    
    if (noProvinceSelected) noProvinceSelected.style.display = 'block';
    if (provinceDetailsContainer) provinceDetailsContainer.style.display = 'none';
}

// ================================
// FUNCIONES GLOBALES (para HTML)
// ================================

// Hacer funciones disponibles globalmente para onclick en HTML
window.selectProvinceFromModal = selectProvinceFromModal;
window.selectProvince = selectProvince;
window.selectQuestion = selectQuestion;
window.editQuestion = editQuestion;
window.confirmDeleteQuestion = confirmDeleteQuestion;
window.editAnswer = editAnswer;
window.confirmDeleteAnswer = confirmDeleteAnswer;
window.editAttraction = editAttraction;
window.confirmDeleteAttraction = confirmDeleteAttraction;
window.editProduct = editProduct;
window.confirmDeleteProduct = confirmDeleteProduct;

// Funciones del quiz
window.showQuizConfigModal = showQuizConfigModal;
window.startQuiz = startQuiz;
window.nextQuestion = nextQuestion;
window.prevQuestion = prevQuestion;
window.finishQuiz = finishQuiz;
window.restartQuiz = restartQuiz;
window.exitQuiz = exitQuiz;

// ================================
// FUNCIONES DE EVENTOS
// ================================



function setupQuizEventListeners() {
    // Botones de configuración del quiz
    const startQuizFromConfigBtn = document.getElementById('startQuizFromConfig');
    const cancelQuizConfigBtn = document.getElementById('cancelQuizConfig');
    
    if (startQuizFromConfigBtn) {
        startQuizFromConfigBtn.addEventListener('click', startQuiz);
    }
    if (cancelQuizConfigBtn) {
        cancelQuizConfigBtn.addEventListener('click', function() {
            const modal = bootstrap.Modal.getInstance(document.getElementById('quizConfigModal'));
            if (modal) modal.hide();
        });
    }
    
    // Botones de navegación del quiz
    const nextBtn = document.getElementById('quizNextBtn');
    const prevBtn = document.getElementById('quizPrevBtn');
    const finishBtn = document.getElementById('quizFinishBtn');
    const restartBtn = document.getElementById('restartQuizBtn');
    const exitBtn = document.getElementById('exitQuizBtn');
    
    if (nextBtn) nextBtn.addEventListener('click', nextQuestion);
    if (prevBtn) prevBtn.addEventListener('click', prevQuestion);
    if (finishBtn) finishBtn.addEventListener('click', finishQuiz);
    if (restartBtn) restartBtn.addEventListener('click', restartQuiz);
    if (exitBtn) exitBtn.addEventListener('click', exitQuiz);
}

// ================================
// FUNCIONES DE BÚSQUEDA Y FILTROS
// ================================

function setupSearchFunctionality() {
    const searchInput = document.getElementById('searchInput');
    const modalSearchInput = document.getElementById('modalSearchInput');
    const regionFilter = document.getElementById('regionFilter');
    
    // Búsqueda en tiempo real
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            filterProvinces(searchTerm);
        });
    }
    
    // Búsqueda en modal
    if (modalSearchInput) {
        modalSearchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const regionFilterValue = document.getElementById('regionFilter')?.value || '';
            updateProvincesModal(searchTerm, regionFilterValue);
        });
    }
    
    // Filtro de región
    if (regionFilter) {
        regionFilter.addEventListener('change', function() {
            const searchTerm = document.getElementById('modalSearchInput')?.value.toLowerCase() || '';
            updateProvincesModal(searchTerm, this.value);
        });
    }
    
    // Configurar filtros de admin
    setupAdminFilters();
}



function showQuizLoading() {
    document.getElementById('quizContent').innerHTML = `
        <div class="quiz-loading text-center py-5">
            <div class="spinner-border text-primary mb-3" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
            <h5>Preparando tu quiz...</h5>
            <p class="text-muted">Cargando preguntas y respuestas</p>
        </div>
    `;
}

function hideQuizLoading() {
}

function showQuizError(message) {
    document.getElementById('quizContent').innerHTML = `
        <div class="text-center py-5">
            <i class="fas fa-exclamation-triangle text-danger mb-3" style="font-size: 3rem;"></i>
            <h5 class="text-danger">Error</h5>
            <p>${message}</p>
            <button class="btn btn-primary" onclick="restartQuiz()">
                <i class="fas fa-redo"></i> Intentar de nuevo
            </button>
        </div>
    `;
    
    // Ocultar botones del footer
    document.getElementById('quizContent-footer').style.display = 'none';
}


// ================================
// GOOGLE MAPS INTEGRATION
// ================================

let map;
let provinceMarkers = [];

// Función que llama Google Maps cuando carga
window.initMap = function() {
    // Centrar el mapa en República Dominicana
    const dominicana = { lat: 18.7357, lng: -70.1627 };
    
    map = new google.maps.Map(document.getElementById('mapContainer'), {
        zoom: 8,
        center: dominicana,
        styles: [
            {
                "featureType": "all",
                "elementType": "geometry.fill",
                "stylers": [{"weight": "2.00"}]
            },
            {
                "featureType": "all",
                "elementType": "geometry.stroke",
                "stylers": [{"color": "#9c9c9c"}]
            },
            {
                "featureType": "all",
                "elementType": "labels.text",
                "stylers": [{"visibility": "on"}]
            }
        ]
    });
    
    // Cargar marcadores cuando el mapa esté listo
    if (allProvinces.length > 0) {
        addProvinceMarkers();
    }
    
    console.log('Google Maps inicializado');
};

function addProvinceMarkers() {
    provinceMarkers.forEach(marker => marker.setMap(null));
    provinceMarkers = [];
    
    // Coordenadas aproximadas de algunas provincias principales
    const provinceCoordinates = {
        1: { lat: 18.4861, lng: -69.9312, name: "Distrito Nacional" },
        9: { lat: 19.4515, lng: -70.6969, name: "Santiago" },
        19: { lat: 18.5204, lng: -68.3740, name: "La Altagracia" },
        20: { lat: 18.4273, lng: -68.9728, name: "La Romana" },
 
    };
    
    allProvinces.forEach(province => {
        const coords = provinceCoordinates[province.id];
        if (coords) {
            const marker = new google.maps.Marker({
                position: { lat: coords.lat, lng: coords.lng },
                map: map,
                title: province.name,
                icon: {
                    url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTEyIDJDOC4xMyAyIDUgNS4xMyA1IDlDNSAxNC4yNSAxMiAyMiAxMiAyMkMxMiAyMiAxOSAxNC4yNSAxOSA5QzE5IDUuMTMgMTUuODcgMiAxMiAyWk0xMiAxMS41QzEwLjYyIDExLjUgOS41IDEwLjM4IDkuNSA5QzkuNSA3LjYyIDEwLjYyIDYuNSAxMiA2LjVDMTMuMzggNi41IDE0LjUgNy42MiAxNC41IDlDMTQuNSAxMC4zOCAxMy4zOCAxMS41IDEyIDExLjVaIiBmaWxsPSIjMzQ5OGRiIi8+Cjwvc3ZnPgo=',
                    scaledSize: new google.maps.Size(30, 30)
                }
            });
            
            marker.addListener('click', () => {
                selectProvince(province.id);
                
                const infoWindow = new google.maps.InfoWindow({
                    content: `
                        <div style="padding: 10px;">
                            <h6 style="margin: 0 0 5px 0; color: #2c3e50;">${province.name}</h6>
                            <p style="margin: 0; color: #7f8c8d;">Capital: ${province.capital}</p>
                            <p style="margin: 0; color: #7f8c8d;">Población: ${formatNumber(province.population)}</p>
                        </div>
                    `
                });
                
                infoWindow.open(map, marker);
            });
            
            provinceMarkers.push(marker);
        }
    });
}

function updateMapMarkers() {
    if (map && allProvinces.length > 0) {
        addProvinceMarkers();
    }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});