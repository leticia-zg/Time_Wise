namespace TimeWise.API.Helpers
{
    /// <summary>
    /// Resultado paginado de uma consulta
    /// </summary>
    /// <typeparam name="T">Tipo dos itens na lista</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Lista de itens da página atual
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        
        /// <summary>
        /// Total de registros disponíveis
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Número da página atual
        /// </summary>
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Tamanho da página (quantidade de itens por página)
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Total de páginas disponíveis
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
