// ========== AUTOCOMPLETAR DATOS EN FORMULARIOS ==========

// Autocompletar código de producto con formato LETRAS-NÚMEROS
function formatearCodigoProducto(input) {
    // Convertir a mayúsculas y validar formato
    let valor = input.value.toUpperCase();
    
    // Eliminar caracteres no permitidos
    valor = valor.replace(/[^A-Z0-9-]/g, '');
    
    // Validar formato LETRAS-NÚMEROS (ejemplo: MED-123)
    const regex = /^([A-Z]+)-?(\d*)$/;
    const match = valor.match(regex);
    
    if (match) {
        const letras = match[1];
        const numeros = match[2];
        
        // Auto-agregar guión si tiene letras y números
        if (numeros && !valor.includes('-')) {
            valor = `${letras}-${numeros}`;
        }
    }
    
    input.value = valor;
    
    // Mostrar feedback visual
    const formatoValido = /^[A-Z]+-\d+$/.test(valor);
    if (valor.length > 0) {
        if (formatoValido) {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        } else {
            input.classList.remove('is-valid');
            input.classList.add('is-invalid');
        }
    } else {
        input.classList.remove('is-valid', 'is-invalid');
    }
}

// Sugerir código basado en categoría
function sugerirCodigoProducto(categoriaSelect, codigoInput) {
    const categoria = categoriaSelect.options[categoriaSelect.selectedIndex].text;
    
    // Mapeo de categorías a prefijos
    const prefijos = {
        'Medicamentos': 'MED',
        'Implementos': 'IMP',
        'Insumos': 'INS',
        'Equipos': 'EQP',
        'Material Quirúrgico': 'MQX',
        'Antibióticos': 'ANT',
        'Analgésicos': 'ANG',
        'Vitaminas': 'VIT'
    };
    
    // Buscar prefijo por coincidencia parcial
    let prefijo = 'PRD'; // Prefijo por defecto
    for (const [key, value] of Object.entries(prefijos)) {
        if (categoria.toLowerCase().includes(key.toLowerCase())) {
            prefijo = value;
            break;
        }
    }
    
    // Si el campo está vacío, sugerir formato
    if (!codigoInput.value) {
        codigoInput.placeholder = `Ejemplo: ${prefijo}-001`;
        codigoInput.setAttribute('data-prefijo', prefijo);
    }
}

// Calcular subtotal automáticamente
function calcularSubtotal(cantidadInput, precioInput, subtotalElement) {
    const cantidad = parseFloat(cantidadInput.value) || 0;
    const precio = parseFloat(precioInput.value) || 0;
    const subtotal = cantidad * precio;
    
    if (subtotalElement.tagName === 'INPUT') {
        subtotalElement.value = subtotal.toFixed(2);
    } else {
        subtotalElement.textContent = `S/ ${subtotal.toFixed(2)}`;
    }
    
    return subtotal;
}

// Calcular total de compra
function calcularTotalCompra() {
    let total = 0;
    document.querySelectorAll('.subtotal-item').forEach(element => {
        const valor = parseFloat(element.textContent.replace('S/', '').trim()) || 
                     parseFloat(element.value) || 0;
        total += valor;
    });
    
    const totalElement = document.getElementById('montoTotal');
    if (totalElement) {
        if (totalElement.tagName === 'INPUT') {
            totalElement.value = total.toFixed(2);
        } else {
            totalElement.textContent = `S/ ${total.toFixed(2)}`;
        }
    }
    
    return total;
}

// Validar stock disponible al seleccionar producto
function validarStockDisponible(productoSelect, cantidadInput) {
    const option = productoSelect.options[productoSelect.selectedIndex];
    const stockDisponible = parseInt(option.getAttribute('data-stock')) || 0;
    const cantidad = parseInt(cantidadInput.value) || 0;
    
    const stockInfo = cantidadInput.parentElement.querySelector('.stock-info');
    if (stockInfo) {
        stockInfo.textContent = `Stock disponible: ${stockDisponible}`;
    }
    
    if (cantidad > stockDisponible) {
        cantidadInput.classList.add('is-invalid');
        const feedback = cantidadInput.parentElement.querySelector('.invalid-feedback') || 
                        createFeedbackElement(cantidadInput);
        feedback.textContent = `Stock insuficiente. Disponible: ${stockDisponible}`;
        return false;
    } else {
        cantidadInput.classList.remove('is-invalid');
        return true;
    }
}

// Crear elemento de feedback
function createFeedbackElement(input) {
    const feedback = document.createElement('div');
    feedback.className = 'invalid-feedback';
    input.parentElement.appendChild(feedback);
    return feedback;
}

// Cargar datos del producto seleccionado
function cargarDatosProducto(productoSelect, contenedorDatos) {
    const option = productoSelect.options[productoSelect.selectedIndex];
    
    const datos = {
        stock: option.getAttribute('data-stock'),
        precio: option.getAttribute('data-precio'),
        unidad: option.getAttribute('data-unidad'),
        categoria: option.getAttribute('data-categoria')
    };
    
    // Mostrar información del producto
    if (contenedorDatos) {
        contenedorDatos.innerHTML = `
            <div class="alert alert-info">
                <strong>Información del producto:</strong><br>
                Stock disponible: ${datos.stock} ${datos.unidad}<br>
                Precio unitario: S/ ${parseFloat(datos.precio).toFixed(2)}<br>
                Categoría: ${datos.categoria}
            </div>
        `;
    }
    
    return datos;
}

// Formatear DNI (solo números, máximo 8)
function formatearDNI(input) {
    let valor = input.value.replace(/\D/g, '');
    if (valor.length > 8) {
        valor = valor.substring(0, 8);
    }
    input.value = valor;
    
    // Validación visual
    if (valor.length === 8) {
        input.classList.remove('is-invalid');
        input.classList.add('is-valid');
    } else if (valor.length > 0) {
        input.classList.remove('is-valid');
        input.classList.add('is-invalid');
    }
}

// Formatear RUC (solo números, máximo 11)
function formatearRUC(input) {
    let valor = input.value.replace(/\D/g, '');
    if (valor.length > 11) {
        valor = valor.substring(0, 11);
    }
    input.value = valor;
    
    // Validación visual
    if (valor.length === 11) {
        input.classList.remove('is-invalid');
        input.classList.add('is-valid');
    } else if (valor.length > 0) {
        input.classList.remove('is-valid');
        input.classList.add('is-invalid');
    }
}

// Formatear teléfono (solo números, máximo 9)
function formatearTelefono(input) {
    let valor = input.value.replace(/\D/g, '');
    if (valor.length > 9) {
        valor = valor.substring(0, 9);
    }
    input.value = valor;
}

// Auto-completar campos al cargar proveedor
function cargarDatosProveedor(proveedorSelect) {
    const option = proveedorSelect.options[proveedorSelect.selectedIndex];
    
    const datos = {
        ruc: option.getAttribute('data-ruc'),
        direccion: option.getAttribute('data-direccion'),
        telefono: option.getAttribute('data-telefono'),
        email: option.getAttribute('data-email')
    };
    
    // Mostrar información del proveedor
    const infoProveedor = document.getElementById('infoProveedor');
    if (infoProveedor) {
        infoProveedor.innerHTML = `
            <div class="card">
                <div class="card-body">
                    <h6 class="card-title">Datos del Proveedor</h6>
                    <p class="mb-1"><strong>RUC:</strong> ${datos.ruc}</p>
                    <p class="mb-1"><strong>Dirección:</strong> ${datos.direccion}</p>
                    <p class="mb-1"><strong>Teléfono:</strong> ${datos.telefono}</p>
                    <p class="mb-0"><strong>Email:</strong> ${datos.email}</p>
                </div>
            </div>
        `;
    }
    
    return datos;
}

// Generar número de comprobante automático
function generarNumeroComprobante(tipoComprobante) {
    const fecha = new Date();
    const año = fecha.getFullYear();
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const dia = String(fecha.getDate()).padStart(2, '0');
    const random = String(Math.floor(Math.random() * 1000)).padStart(3, '0');
    
    const prefijo = tipoComprobante === 'Factura' ? 'F' : 
                   tipoComprobante === 'Boleta' ? 'B' : 'C';
    
    return `${prefijo}${año}${mes}${dia}-${random}`;
}

// Inicializar todos los eventos al cargar la página
document.addEventListener('DOMContentLoaded', function() {
    
    // Formateo de código de producto
    const codigoProductoInput = document.getElementById('CodigoProducto');
    if (codigoProductoInput) {
        codigoProductoInput.addEventListener('input', function() {
            formatearCodigoProducto(this);
        });
        
        // Sugerir código basado en categoría
        const categoriaSelect = document.getElementById('CategoriaId');
        if (categoriaSelect) {
            categoriaSelect.addEventListener('change', function() {
                sugerirCodigoProducto(this, codigoProductoInput);
            });
        }
    }
    
    // Formateo de DNI
    const dniInput = document.getElementById('DNI');
    if (dniInput) {
        dniInput.addEventListener('input', function() {
            formatearDNI(this);
        });
    }
    
    // Formateo de RUC
    const rucInput = document.getElementById('RUC');
    if (rucInput) {
        rucInput.addEventListener('input', function() {
            formatearRUC(this);
        });
    }
    
    // Formateo de teléfono
    const telefonoInputs = document.querySelectorAll('input[type="tel"], input[id*="Telefono"]');
    telefonoInputs.forEach(input => {
        input.addEventListener('input', function() {
            formatearTelefono(this);
        });
    });
    
    // Generar número de comprobante
    const tipoComprobanteSelect = document.getElementById('TipoComprobante');
    const numeroComprobanteInput = document.getElementById('NumeroComprobante');
    if (tipoComprobanteSelect && numeroComprobanteInput) {
        tipoComprobanteSelect.addEventListener('change', function() {
            if (!numeroComprobanteInput.value) {
                numeroComprobanteInput.value = generarNumeroComprobante(this.value);
            }
        });
    }
    
    console.log('✅ Scripts de autocompletado cargados correctamente');
});